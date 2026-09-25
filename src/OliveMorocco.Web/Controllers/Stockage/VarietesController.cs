using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Business.Services.Stockage;
using OliveMorocco.Web.Models.Stockage.Varietes;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class VarietesController(IVarieteService varietes) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await varietes.GetVarietesAsync(
            search,
            page,
            VarieteListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new VarieteListViewModel
        {
            Items = result.Items,
            Search = Normalize(search),
            Page = page,
            TotalCount = result.TotalCount,
        });
    }

    [HttpGet("Create")]
    public IActionResult Create() => View(new VarieteFormViewModel());

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        VarieteFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await varietes.CreateVarieteAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(model);
        }

        TempData["Success"] = "Variété enregistrée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var variete = await varietes.GetVarieteByIdAsync(id, cancellationToken);
        if (variete is null)
            return NotFound();

        return View(ToFormViewModel(variete));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        VarieteFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.Id = id;

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await varietes.UpdateVarieteAsync(id, ToUpdateDto(model), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(model);
        }

        TempData["Success"] = "Variété modifiée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await varietes.DeleteVarieteAsync(id, cancellationToken);
            TempData["Success"] = "Variété supprimée avec succès.";
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

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    private static VarieteFormViewModel ToFormViewModel(VarieteDto variete) =>
        new()
        {
            Id = variete.Id,
            Nom = variete.Nom,
            Code = variete.Code,
            RegionOrigine = variete.RegionOrigine,
        };

    private static CreateVarieteDto ToCreateDto(VarieteFormViewModel model) =>
        new(
            model.Nom.Trim(),
            NormalizeOptional(model.Code),
            NormalizeOptional(model.RegionOrigine));

    private static UpdateVarieteDto ToUpdateDto(VarieteFormViewModel model) =>
        new(
            model.Nom.Trim(),
            NormalizeOptional(model.Code),
            NormalizeOptional(model.RegionOrigine));

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
