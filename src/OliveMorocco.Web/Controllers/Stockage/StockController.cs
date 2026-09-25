using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Business.Services.Stockage;
using OliveMorocco.Web.Models.Stockage.Stock;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class StockController(IStockService stock) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        bool stockBasOnly = false,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await stock.GetStockEtatAsync(
            search,
            stockBasOnly,
            page,
            StockListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new StockListViewModel
        {
            Items = result.Items,
            Search = Normalize(search),
            StockBasOnly = stockBasOnly,
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

        var produit = await stock.GetProduitStockDetailAsync(id, cancellationToken);
        if (produit is null)
            return NotFound();

        var mouvements = await stock.GetMouvementsAsync(
            id,
            page,
            StockDetailViewModel.DefaultMouvementPageSize,
            cancellationToken);

        return View(new StockDetailViewModel
        {
            Produit = produit,
            Mouvements = mouvements.Items,
            MouvementPage = page,
            MouvementTotalCount = mouvements.TotalCount,
        });
    }

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
            await stock.CreateAjustementAsync(
                id,
                new CreateAjustementStockDto(model.Variation!.Value, model.Note),
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
        StockAjustementViewModel formState,
        CancellationToken cancellationToken)
    {
        var produit = await stock.GetProduitStockDetailAsync(id, cancellationToken);
        if (produit is null)
            return NotFound();

        var mouvements = await stock.GetMouvementsAsync(
            id,
            page,
            StockDetailViewModel.DefaultMouvementPageSize,
            cancellationToken);

        return View(nameof(Detail), new StockDetailViewModel
        {
            Produit = produit,
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
