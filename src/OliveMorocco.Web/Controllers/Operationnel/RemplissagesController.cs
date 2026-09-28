using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Business.Services.Operationnel;
using OliveMorocco.Web.Models.Operationnel.Remplissages;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Operationnel;

[Route(AppSections.Operationnel + "/[controller]")]
public sealed class RemplissagesController(IRemplissageService remplissages) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await remplissages.GetRemplissagesAsync(
            search,
            page,
            RemplissageListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new RemplissageListViewModel
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
        RemplissageFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        RemoveEmptyLines(model);

        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        RemplissageDto created;
        try
        {
            created = await remplissages.CreateRemplissageAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        TempData["Success"] = $"Remplissage {created.Numero} enregistré avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var remplissage = await remplissages.GetRemplissageByIdAsync(id, cancellationToken);
        if (remplissage is null)
            return NotFound();

        return View(await BuildFormAsync(ToFormViewModel(remplissage), cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        RemplissageFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.Id = id;
        RemoveEmptyLines(model);

        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await remplissages.UpdateRemplissageAsync(id, ToUpdateDto(model), cancellationToken);
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

        TempData["Success"] = "Remplissage modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await remplissages.DeleteRemplissageAsync(id, cancellationToken);
            TempData["Success"] = "Remplissage supprimé avec succès.";
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

    private async Task<RemplissageFormViewModel> BuildFormAsync(
        RemplissageFormViewModel? model = null,
        CancellationToken cancellationToken = default)
    {
        model ??= new RemplissageFormViewModel { Date = DateTime.Today };

        if (model.IsEdit)
        {
            var existing = await remplissages.GetRemplissageByIdAsync(model.Id!.Value, cancellationToken);
            if (existing is not null)
            {
                model.Numero = existing.Numero;
                model.VarieteIdInitiale = existing.VarieteId;
                model.QuantiteHuileInitiale = existing.QuantiteHuile;
            }
        }

        model.Varietes = await remplissages.GetVarietesForSelectAsync(cancellationToken);
        model.Produits = await remplissages.GetProduitsForSelectAsync(cancellationToken);
        return model;
    }

    private void RemoveEmptyLines(RemplissageFormViewModel model)
    {
        model.Lignes = model.Lignes
            .Where(l => l.ProduitId > 0 || l.Quantite > 0)
            .ToList();

        foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Lignes[", StringComparison.Ordinal)).ToList())
            ModelState.Remove(key);
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
        {
            var key = error.PropertyName.StartsWith("Lignes", StringComparison.Ordinal)
                ? string.Empty
                : error.PropertyName;
            ModelState.AddModelError(key, error.ErrorMessage);
        }
    }

    private static RemplissageFormViewModel ToFormViewModel(RemplissageDto remplissage) =>
        new()
        {
            Id = remplissage.Id,
            Numero = remplissage.Numero,
            VarieteId = remplissage.VarieteId,
            Date = remplissage.Date,
            Perte = remplissage.Perte,
            Note = remplissage.Note,
            Lignes = remplissage.Lignes
                .Select(l => new RemplissageLigneViewModel { ProduitId = l.ProduitId, Quantite = l.Quantite })
                .ToList(),
        };

    private static CreateRemplissageDto ToCreateDto(RemplissageFormViewModel model) =>
        new(
            model.VarieteId,
            model.Date,
            model.Perte,
            model.Note,
            model.Lignes.Select(l => new CreateRemplissageLigneDto(l.ProduitId, l.Quantite)).ToList());

    private static UpdateRemplissageDto ToUpdateDto(RemplissageFormViewModel model) =>
        new(
            model.VarieteId,
            model.Date,
            model.Perte,
            model.Note,
            model.Lignes.Select(l => new CreateRemplissageLigneDto(l.ProduitId, l.Quantite)).ToList());

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
