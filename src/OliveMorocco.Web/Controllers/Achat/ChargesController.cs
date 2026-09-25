using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.Services.Achat;
using OliveMorocco.Web.Models.Achat.Charges;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class ChargesController(IChargeService charges) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await charges.GetChargesAsync(
            search,
            page,
            ChargeListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new ChargeListViewModel
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
        ChargeFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await charges.CreateChargeAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        TempData["Success"] = "Charge enregistrée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var charge = await charges.GetChargeByIdAsync(id, cancellationToken);
        if (charge is null)
            return NotFound();

        if (charge.InterventionId.HasValue)
            return RedirectToAction(nameof(Index));

        return View(await BuildFormAsync(ToFormViewModel(charge), cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ChargeFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.Id = id;

        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await charges.UpdateChargeAsync(id, ToUpdateDto(model), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException)
        {
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        TempData["Success"] = "Charge modifiée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await charges.DeleteChargeAsync(id, cancellationToken);
            TempData["Success"] = "Charge supprimée avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task<ChargeFormViewModel> BuildFormAsync(
        ChargeFormViewModel? model = null,
        CancellationToken cancellationToken = default)
    {
        var types = await charges.GetActiveTypesAsync(cancellationToken);

        if (model is null)
        {
            return new ChargeFormViewModel
            {
                Date = DateTime.Today,
                TypeCharges = types,
            };
        }

        model.TypeCharges = types;
        return model;
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    private static ChargeFormViewModel ToFormViewModel(ChargeDto charge) =>
        new()
        {
            Id = charge.Id,
            TypeChargeId = charge.TypeChargeId,
            Libelle = charge.Libelle,
            Date = charge.Date,
            MontantTtc = charge.MontantTtc,
            Note = charge.Note,
        };

    private static CreateChargeDto ToCreateDto(ChargeFormViewModel model) =>
        new(
            model.TypeChargeId,
            model.Libelle.Trim(),
            model.Date,
            model.MontantTtc,
            TrimOrEmpty(model.Note));

    private static UpdateChargeDto ToUpdateDto(ChargeFormViewModel model) =>
        new(
            model.TypeChargeId,
            model.Libelle.Trim(),
            model.Date,
            model.MontantTtc,
            TrimOrEmpty(model.Note));

    private static string TrimOrEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
