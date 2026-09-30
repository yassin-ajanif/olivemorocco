using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Business.Services.Stockage.Produits;
using OliveMorocco.Web.Models.Stockage.Produits;
using OliveMorocco.Web.Photos;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage.Produits;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class ProduitsController(IProduitService produits, IPhotoStore photos) : Controller
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

        // Saved before the product row so the URL can go into the DTO. If the row then
        // fails validation the file is rolled back, otherwise a rejected form would leave
        // an orphan in the photo directory.
        var stored = await TryStoreUploadAsync(model, cancellationToken);
        if (stored is not null && !stored.Succeeded)
        {
            ModelState.AddModelError(nameof(model.ImageFile), stored.Erreur!);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        // A new product has no photo on file yet, so the URL comes from the upload alone.
        var imageUrl = stored is { Succeeded: true } ? stored.Url : null;

        try
        {
            await produits.CreateProduitAsync(ToCreateDto(model, imageUrl), cancellationToken);
        }
        catch (Exception exception)
        {
            // Broad on purpose. The upload is already on disk at this point, so any
            // failure here must roll it back — a DbUpdateException or a dropped
            // connection would otherwise strand the file with nothing pointing at it.
            // This is deliberately not part of the exception branching below: it depends
            // on whether a file was written, not on why the save failed.
            if (stored is { Succeeded: true })
                photos.Delete(stored.Url);

            if (exception is ValidationException validation)
            {
                AddValidationErrors(validation);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Enregistrement impossible. Réessayez.");
            }

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

        // Read before the update so the replaced file can be removed afterwards. This is
        // a second read on every edit; taking the old URL from a hidden field instead
        // would let a caller name any file to delete.
        var previousUrl = (await produits.GetProduitByIdAsync(id, cancellationToken))?.ImageUrl;

        var stored = await TryStoreUploadAsync(model, cancellationToken);
        if (stored is not null && !stored.Succeeded)
        {
            ModelState.AddModelError(nameof(model.ImageFile), stored.Erreur!);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        // Upload wins over the remove flag, so picking a file after ticking the box
        // still ends up with a photo rather than silently doing nothing.
        var imageUrl = stored is { Succeeded: true }
            ? stored.Url
            : model.RemoveImage
                ? null
                : previousUrl;

        // The preview is display-only and is not posted back, so restore it for the
        // error re-renders below. Without this a rejected form would show an empty
        // slot even though the product still has its photo.
        model.CurrentImageUrl = previousUrl;

        try
        {
            await produits.UpdateProduitAsync(id, ToUpdateDto(model, imageUrl), cancellationToken);
        }
        catch (Exception exception)
        {
            if (stored is { Succeeded: true })
                photos.Delete(stored.Url);

            if (exception is KeyNotFoundException)
                return NotFound();

            if (exception is ValidationException validation)
            {
                AddValidationErrors(validation);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Enregistrement impossible. Réessayez.");
            }

            return View(await BuildFormAsync(model, cancellationToken));
        }

        // Only now is the old file unreferenced. Comparing against previousUrl rather
        // than checking for an upload also covers the remove case.
        if (imageUrl != previousUrl)
            photos.Delete(previousUrl);

        TempData["Success"] = "Produit modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            // Grab the URL first: the delete removes the row we would read it from.
            var imageUrl = (await produits.GetProduitByIdAsync(id, cancellationToken))?.ImageUrl;

            await produits.DeleteProduitAsync(id, cancellationToken);

            // The file goes only once the row is really gone. DeleteProduitAsync throws
            // when documents still reference the product, and an image must not vanish
            // because a delete was merely refused.
            photos.Delete(imageUrl);

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

    /// <summary>
    /// Writes the uploaded file, if any. Returns null when the form carried no upload,
    /// which is the common case — the caller then falls back to the photo already on
    /// file, or to the remove flag.
    /// </summary>
    private async Task<PhotoSaveResult?> TryStoreUploadAsync(
        ProduitFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.ImageFile is not { Length: > 0 })
            return null;

        return await photos.SaveAsync(model.ImageFile, cancellationToken);
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
            StockMinimum = produit.StockMinimum,
            Actif = produit.Actif,
            ContenanceLitres = produit.ContenanceLitres,
            CurrentImageUrl = produit.ImageUrl,
        };

    private static CreateProduitDto ToCreateDto(ProduitFormViewModel model, string? imageUrl) =>
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
            model.Actif,
            model.ContenanceLitres,
            imageUrl);

    private static UpdateProduitDto ToUpdateDto(ProduitFormViewModel model, string? imageUrl) =>
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
            model.Actif,
            model.ContenanceLitres,
            imageUrl);

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
