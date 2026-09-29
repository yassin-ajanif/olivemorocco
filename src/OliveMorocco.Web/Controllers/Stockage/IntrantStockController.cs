using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Business.Services.Stockage.Intrants;
using OliveMorocco.Web.Models.Stockage.IntrantStock;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class IntrantStockController(IStockIntrantService stock) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await stock.GetStockIntrantAsync(
            search,
            page,
            IntrantStockListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new IntrantStockListViewModel
        {
            Items = result.Items,
            Search = Normalize(search),
            Page = page,
            TotalCount = result.TotalCount,
        });
    }

    [HttpGet("Detail/{id:int}")]
    public async Task<IActionResult> Detail(
        int id,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var intrant = await stock.GetDetailAsync(id, cancellationToken);
        if (intrant is null)
            return NotFound();

        var mouvements = await stock.GetMouvementsAsync(
            id,
            page,
            IntrantStockDetailViewModel.DefaultMouvementPageSize,
            cancellationToken);

        return View(new IntrantStockDetailViewModel
        {
            Intrant = intrant,
            Mouvements = mouvements.Items,
            MouvementPage = page,
            MouvementTotalCount = mouvements.TotalCount,
        });
    }

    [HttpPost("Detail/{id:int}/Ajustement")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ajustement(
        int id,
        [Bind(Prefix = "Ajustement")] IntrantStockAjustementViewModel model,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        const string prefix = "Ajustement";

        if (!model.Variation.HasValue)
        {
            ModelState.AddModelError(
                $"{prefix}.{nameof(IntrantStockAjustementViewModel.Variation)}",
                "La variation est requise.");
        }

        if (!ModelState.IsValid)
            return await DetailViewAsync(id, page, model, cancellationToken);

        try
        {
            await stock.CreateAjustementAsync(
                id,
                new CreateAjustementIntrantDto(model.Variation!.Value, model.Note),
                cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception, prefix);
            return await DetailViewAsync(id, page, model, cancellationToken);
        }

        TempData["Success"] = "Stock ajusté avec succès.";
        return RedirectToAction(nameof(Detail), new { id, page = 1 });
    }

    private async Task<IActionResult> DetailViewAsync(
        int id,
        int page,
        IntrantStockAjustementViewModel formState,
        CancellationToken cancellationToken)
    {
        var intrant = await stock.GetDetailAsync(id, cancellationToken);
        if (intrant is null)
            return NotFound();

        var mouvements = await stock.GetMouvementsAsync(
            id,
            page,
            IntrantStockDetailViewModel.DefaultMouvementPageSize,
            cancellationToken);

        return View(nameof(Detail), new IntrantStockDetailViewModel
        {
            Intrant = intrant,
            Mouvements = mouvements.Items,
            MouvementPage = page,
            MouvementTotalCount = mouvements.TotalCount,
            Ajustement = formState,
        });
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void AddValidationErrors(ValidationException exception, string? keyPrefix = null)
    {
        foreach (var error in exception.Errors)
        {
            var key = string.IsNullOrEmpty(error.PropertyName)
                ? string.Empty
                : keyPrefix is null
                    ? error.PropertyName
                    : $"{keyPrefix}.{error.PropertyName}";

            ModelState.AddModelError(key, error.ErrorMessage);
        }
    }
}
