using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Business.Services.Stockage;
using OliveMorocco.Web.Models.Stockage.HuileVrac;
using OliveMorocco.Web.Models.Stockage.Stock;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class HuileVracController(IStockHuileService stockHuile) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await stockHuile.GetStockHuileAsync(
            search,
            page,
            HuileVracListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new HuileVracListViewModel
        {
            Items = result.Items,
            Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            Page = page,
            TotalCount = result.TotalCount,
        });
    }

    [HttpGet("Detail/{id:int}")]
    public async Task<IActionResult> Detail(
        int id,
        int page = 1,
        CancellationToken cancellationToken = default)
        => await DetailViewAsync(id, Math.Max(1, page), new StockAjustementViewModel(), cancellationToken);

    [HttpPost("Detail/{id:int}/Ajustement")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ajustement(
        int id,
        [Bind(Prefix = "Ajustement")] StockAjustementViewModel model,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        const string prefix = "Ajustement";

        if (!model.Variation.HasValue)
        {
            ModelState.AddModelError(
                $"{prefix}.{nameof(StockAjustementViewModel.Variation)}",
                "La variation est requise.");
        }

        if (!ModelState.IsValid)
            return await DetailViewAsync(id, page, model, cancellationToken);

        try
        {
            await stockHuile.CreateAjustementAsync(
                id,
                new CreateAjustementStockDto(model.Variation!.Value, model.Note),
                cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException exception)
        {
            foreach (var error in exception.Errors)
            {
                var key = string.IsNullOrEmpty(error.PropertyName)
                    ? string.Empty
                    : $"{prefix}.{error.PropertyName}";
                ModelState.AddModelError(key, error.ErrorMessage);
            }

            return await DetailViewAsync(id, page, model, cancellationToken);
        }

        TempData["Success"] = "Stock d'huile ajusté avec succès.";
        return RedirectToAction(nameof(Detail), new { id, page = 1 });
    }

    private async Task<IActionResult> DetailViewAsync(
        int id,
        int page,
        StockAjustementViewModel formState,
        CancellationToken cancellationToken)
    {
        var variete = await stockHuile.GetDetailAsync(id, cancellationToken);
        if (variete is null)
            return NotFound();

        var mouvements = await stockHuile.GetMouvementsAsync(
            id,
            page,
            HuileVracDetailViewModel.DefaultMouvementPageSize,
            cancellationToken);

        return View(nameof(Detail), new HuileVracDetailViewModel
        {
            Variete = variete,
            Mouvements = mouvements.Items,
            MouvementPage = page,
            MouvementTotalCount = mouvements.TotalCount,
            Ajustement = formState,
        });
    }
}
