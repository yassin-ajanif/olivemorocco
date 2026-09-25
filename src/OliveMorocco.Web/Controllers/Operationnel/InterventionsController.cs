using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Business.Services.Operationnel;
using OliveMorocco.Web.Models.Operationnel.Interventions;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Operationnel;

[Route(AppSections.Operationnel + "/[controller]")]
public sealed class InterventionsController(IInterventionService interventions) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await interventions.GetInterventionsAsync(
            search,
            page,
            InterventionListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new InterventionListViewModel
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
        InterventionFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await interventions.CreateInterventionAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        TempData["Success"] = "Intervention enregistrée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var intervention = await interventions.GetInterventionByIdAsync(id, cancellationToken);
        if (intervention is null)
            return NotFound();

        return View(await BuildFormAsync(ToFormViewModel(intervention), cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        InterventionFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.Id = id;

        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await interventions.UpdateInterventionAsync(id, ToUpdateDto(model), cancellationToken);
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

        TempData["Success"] = "Intervention modifiée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var intervention = await interventions.GetInterventionByIdAsync(id, cancellationToken);
        if (intervention is null)
            return NotFound();

        await interventions.DeleteInterventionAsync(id, cancellationToken);
        TempData["Success"] = "Intervention supprimée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<InterventionFormViewModel> BuildFormAsync(
        InterventionFormViewModel? model = null,
        CancellationToken cancellationToken = default)
    {
        var secteurs = await interventions.GetSecteursForSelectAsync(cancellationToken);
        var intrants = await interventions.GetIntrantsForSelectAsync(cancellationToken);

        if (model is null)
        {
            return new InterventionFormViewModel
            {
                Date = DateTime.Today,
                Secteurs = secteurs,
                Intrants = intrants,
            };
        }

        model.Secteurs = secteurs;
        model.Intrants = intrants;
        return model;
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    private static InterventionFormViewModel ToFormViewModel(InterventionDto intervention) =>
        new()
        {
            Id = intervention.Id,
            SecteurId = intervention.SecteurId,
            SecteurNom = intervention.SecteurNom,
            Date = intervention.Date,
            IntrantId = intervention.IntrantId,
            QuantiteIntrant = intervention.QuantiteIntrant,
            QuantiteEau = intervention.QuantiteEau,
            Note = intervention.Note,
            TotalCharges = intervention.TotalCharges,
            LinkedCharges = intervention.Charges,
        };

    private static CreateInterventionDto ToCreateDto(InterventionFormViewModel model) =>
        new(
            model.SecteurId,
            model.Date,
            NormalizeIntrantId(model.IntrantId),
            model.QuantiteIntrant,
            model.QuantiteEau,
            TrimOrNull(model.Note));

    private static UpdateInterventionDto ToUpdateDto(InterventionFormViewModel model) =>
        new(
            model.SecteurId,
            model.Date,
            NormalizeIntrantId(model.IntrantId),
            model.QuantiteIntrant,
            model.QuantiteEau,
            TrimOrNull(model.Note));

    private static int? NormalizeIntrantId(int? intrantId) =>
        intrantId is null or 0 ? null : intrantId;

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
