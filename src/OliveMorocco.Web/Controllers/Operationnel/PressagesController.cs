using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Business.Services.Operationnel;
using OliveMorocco.Web.Models.Operationnel.Pressages;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Operationnel;

[Route(AppSections.Operationnel + "/[controller]")]
public sealed class PressagesController(IPressageService pressages) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await pressages.GetPressagesAsync(
            search,
            page,
            PressageListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new PressageListViewModel
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
        PressageFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await pressages.CreatePressageAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        TempData["Success"] = "Pressage enregistré avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var pressage = await pressages.GetPressageByIdAsync(id, cancellationToken);
        if (pressage is null)
            return NotFound();

        return View(await BuildFormAsync(ToFormViewModel(pressage), cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        PressageFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.Id = id;

        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await pressages.UpdatePressageAsync(id, ToUpdateDto(model), cancellationToken);
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

        TempData["Success"] = "Pressage modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await pressages.DeletePressageAsync(id, cancellationToken);
            TempData["Success"] = "Pressage supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("Factures")]
    public async Task<IActionResult> Factures(
        int fournisseurId,
        CancellationToken cancellationToken = default)
    {
        var items = await pressages.GetFacturesForSelectAsync(fournisseurId, cancellationToken);
        return Json(items);
    }

    private async Task<PressageFormViewModel> BuildFormAsync(
        PressageFormViewModel? model = null,
        CancellationToken cancellationToken = default)
    {
        var fournisseurs = await pressages.GetFournisseursForSelectAsync(cancellationToken);
        var varietes = await pressages.GetVarietesForSelectAsync(cancellationToken);

        if (model is null)
        {
            return new PressageFormViewModel
            {
                Date = DateTime.Today,
                Fournisseurs = fournisseurs,
                Varietes = varietes,
            };
        }

        model.Fournisseurs = fournisseurs;
        model.Varietes = varietes;
        model.Factures = model.FournisseurId > 0
            ? await pressages.GetFacturesForSelectAsync(model.FournisseurId, cancellationToken)
            : [];

        return model;
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    private static PressageFormViewModel ToFormViewModel(PressageDto pressage) =>
        new()
        {
            Id = pressage.Id,
            FournisseurId = pressage.FournisseurId,
            VarieteId = pressage.VarieteId,
            Date = pressage.Date,
            QuantiteOlives = pressage.QuantiteOlives,
            Rendement = pressage.Rendement,
            QuantiteHuile = pressage.QuantiteHuile,
            FactureFournisseurId = pressage.FactureFournisseurId,
        };

    private static CreatePressageDto ToCreateDto(PressageFormViewModel model) =>
        new(
            model.FournisseurId,
            model.VarieteId,
            model.Date,
            model.QuantiteOlives,
            model.Rendement,
            model.QuantiteHuile,
            model.FactureFournisseurId);

    private static UpdatePressageDto ToUpdateDto(PressageFormViewModel model) =>
        new(
            model.FournisseurId,
            model.VarieteId,
            model.Date,
            model.QuantiteOlives,
            model.Rendement,
            model.QuantiteHuile,
            model.FactureFournisseurId);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
