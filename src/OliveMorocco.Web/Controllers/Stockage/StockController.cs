using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Business.Services.Stockage;
using OliveMorocco.Business.Services.Stockage.Intrants;
using OliveMorocco.Business.Services.Stockage.Produits;
using OliveMorocco.Web.Models.Stockage.IntrantStock;
using OliveMorocco.Web.Models.Stockage.Stock;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class StockController(IStockProduitService stock, IStockHuileService stockHuile, IStockIntrantService stockIntrant) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        string? vue,
        bool stockBasOnly = false,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        if (string.Equals(vue, StockListViewModel.VueHuile, StringComparison.OrdinalIgnoreCase))
        {
            var huile = await stockHuile.GetStockHuileAsync(
                search,
                page,
                StockListViewModel.DefaultPageSize,
                cancellationToken);

            return View(new StockListViewModel
            {
                Vue = StockListViewModel.VueHuile,
                HuileItems = huile.Items,
                Search = Normalize(search),
                Page = page,
                TotalCount = huile.TotalCount,
            });
        }

        if (string.Equals(vue, StockListViewModel.VueIntrant, StringComparison.OrdinalIgnoreCase))
        {
            var intrant = await stockIntrant.GetStockIntrantAsync(
                search,
                page,
                StockListViewModel.DefaultPageSize,
                cancellationToken);

            return View(new StockListViewModel
            {
                Vue = StockListViewModel.VueIntrant,
                IntrantItems = intrant.Items,
                Search = Normalize(search),
                Page = page,
                TotalCount = intrant.TotalCount,
            });
        }

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

    /// <summary>
    /// Legacy entry point: the standalone intrant stock list is now the « Intrants » tab of this page.
    /// </summary>
    [HttpGet("/" + AppSections.Stockage + "/IntrantStock")]
    public IActionResult LegacyIntrantStock()
        => RedirectToAction(nameof(Index), new { vue = StockListViewModel.VueIntrant });

    [HttpGet("Produit/{id:int}")]
    public async Task<IActionResult> Produit(
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

    [HttpPost("Produit/{id:int}/Ajustement")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProduitAjustement(
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
            return await ProduitViewAsync(id, page, model, cancellationToken);

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
            return await ProduitViewAsync(id, page, model, cancellationToken);
        }

        TempData["Success"] = "Stock ajusté avec succès.";
        return RedirectToAction(nameof(Produit), new { id, page = 1 });
    }

    private async Task<IActionResult> ProduitViewAsync(
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

        return View(nameof(Produit), new StockDetailViewModel
        {
            Produit = produit,
            Mouvements = mouvements.Items,
            MouvementPage = page,
            MouvementTotalCount = mouvements.TotalCount,
            Ajustement = formState,
        });
    }

    [HttpGet("Huile/{id:int}")]
    public async Task<IActionResult> Huile(
        int id,
        int page = 1,
        CancellationToken cancellationToken = default)
        => await HuileViewAsync(id, Math.Max(1, page), new StockAjustementViewModel(), cancellationToken);

    [HttpPost("Huile/{id:int}/Ajustement")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HuileAjustement(
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
            return await HuileViewAsync(id, page, model, cancellationToken);

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
            AddValidationErrors(exception, prefix);
            return await HuileViewAsync(id, page, model, cancellationToken);
        }

        TempData["Success"] = "Stock d'huile ajusté avec succès.";
        return RedirectToAction(nameof(Huile), new { id, page = 1 });
    }

    private async Task<IActionResult> HuileViewAsync(
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
            StockHuileDetailViewModel.DefaultMouvementPageSize,
            cancellationToken);

        return View(nameof(Huile), new StockHuileDetailViewModel
        {
            Variete = variete,
            Mouvements = mouvements.Items,
            MouvementPage = page,
            MouvementTotalCount = mouvements.TotalCount,
            Ajustement = formState,
        });
    }

    [HttpGet("Intrant/{id:int}")]
    public async Task<IActionResult> Intrant(
        int id,
        int page = 1,
        CancellationToken cancellationToken = default)
        => await IntrantViewAsync(id, Math.Max(1, page), new IntrantStockAjustementViewModel(), cancellationToken);

    [HttpPost("Intrant/{id:int}/Ajustement")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IntrantAjustement(
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
            return await IntrantViewAsync(id, page, model, cancellationToken);

        try
        {
            await stockIntrant.CreateAjustementAsync(
                id,
                new CreateAjustementIntrantDto(model.Variation!.Value, model.Note),
                cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception, prefix);
            return await IntrantViewAsync(id, page, model, cancellationToken);
        }

        TempData["Success"] = "Stock d'intrant ajusté avec succès.";
        return RedirectToAction(nameof(Intrant), new { id, page = 1 });
    }

    private async Task<IActionResult> IntrantViewAsync(
        int id,
        int page,
        IntrantStockAjustementViewModel formState,
        CancellationToken cancellationToken)
    {
        var intrant = await stockIntrant.GetDetailAsync(id, cancellationToken);
        if (intrant is null)
            return NotFound();

        var mouvements = await stockIntrant.GetMouvementsAsync(
            id,
            page,
            IntrantStockDetailViewModel.DefaultMouvementPageSize,
            cancellationToken);

        return View(nameof(Intrant), new IntrantStockDetailViewModel
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
