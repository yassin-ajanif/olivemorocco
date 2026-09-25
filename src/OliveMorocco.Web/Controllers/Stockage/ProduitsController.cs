using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Business.Services.Stockage;
using OliveMorocco.Web.Models.Stockage.Produits;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class ProduitsController(
    IProduitService produits,
    IVarieteService varietes) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await produits.GetProduitsAsync(
            search,
            page,
            ProduitListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new ProduitListViewModel
        {
            Items = result.Items,
            Search = Normalize(search),
            Page = page,
            TotalCount = result.TotalCount,
        });
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        => View(await BuildFormAsync(cancellationToken: cancellationToken));

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ProduitFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await produits.CreateProduitAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        TempData["Success"] = "Produit enregistré avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var produit = await produits.GetProduitByIdAsync(id, cancellationToken);
        if (produit is null)
            return NotFound();

        return View(await BuildFormAsync(ToFormViewModel(produit), cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ProduitFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.Id = id;

        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await produits.UpdateProduitAsync(id, ToUpdateDto(model), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        TempData["Success"] = "Produit modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await produits.DeleteProduitAsync(id, cancellationToken);
            TempData["Success"] = "Produit supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException exception)
        {
            TempData["ErrorTitle"] = "Suppression impossible";
            TempData["Error"] = exception.Errors.FirstOrDefault()?.ErrorMessage ?? exception.Message;
            return RedirectToAction(nameof(Edit), new { id });
        }
    }

    [HttpPost("Varietes")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVariete(
        [FromForm] string nom,
        [FromForm] string? code,
        [FromForm] string? regionOrigine,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = new CreateVarieteDto(
                nom?.Trim() ?? string.Empty,
                NormalizeOptional(code),
                NormalizeOptional(regionOrigine));

            var created = await varietes.CreateVarieteAsync(dto, cancellationToken);
            return Json(new { id = created.Id, nom = created.Nom });
        }
        catch (ValidationException exception)
        {
            return BadRequest(new
            {
                error = exception.Errors.FirstOrDefault()?.ErrorMessage ?? "Données invalides.",
            });
        }
    }

    [HttpPost("ToggleActif")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActif(
        int id,
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var produit = await produits.ToggleActifAsync(id, cancellationToken);
            TempData["Success"] = produit.Actif
                ? $"{produit.Designation} a été activé."
                : $"{produit.Designation} a été désactivé.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index), new { search, page });
    }

    private async Task<ProduitFormViewModel> BuildFormAsync(
        ProduitFormViewModel? model = null,
        CancellationToken cancellationToken = default)
    {
        var varietes = await produits.GetVarietesForSelectAsync(cancellationToken);

        if (model is null)
        {
            return new ProduitFormViewModel
            {
                Varietes = varietes,
            };
        }

        model.Varietes = varietes;
        return model;
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    private static ProduitFormViewModel ToFormViewModel(ProduitDto produit) =>
        new()
        {
            Id = produit.Id,
            Reference = produit.Reference,
            Designation = produit.Designation,
            VarieteId = produit.VarieteId,
            Unite = produit.Unite,
            CodeBarre = produit.CodeBarre,
            PrixAchatHT = produit.PrixAchatHT,
            PrixVenteHT = produit.PrixVenteHT,
            TauxTVA = produit.TauxTVA,
            StockActuel = produit.StockActuel,
            StockMinimum = produit.StockMinimum,
            Actif = produit.Actif,
        };

    private static CreateProduitDto ToCreateDto(ProduitFormViewModel model) =>
        new(
            model.Reference.Trim(),
            model.Designation.Trim(),
            model.VarieteId,
            model.Unite.Trim(),
            NormalizeOptional(model.CodeBarre),
            model.PrixAchatHT,
            model.PrixVenteHT,
            model.TauxTVA,
            model.StockInitial,
            model.StockMinimum,
            model.Actif);

    private static UpdateProduitDto ToUpdateDto(ProduitFormViewModel model) =>
        new(
            model.Reference.Trim(),
            model.Designation.Trim(),
            model.VarieteId,
            model.Unite.Trim(),
            NormalizeOptional(model.CodeBarre),
            model.PrixAchatHT,
            model.PrixVenteHT,
            model.TauxTVA,
            model.StockMinimum,
            model.Actif);

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
