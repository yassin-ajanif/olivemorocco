using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Business.Services.Operationnel;
using OliveMorocco.Web.Models.Stockage.Intrants;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class IntrantsController(IIntrantService intrants) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await intrants.GetIntrantsAsync(
            search,
            page,
            IntrantListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new IntrantListViewModel
        {
            Items = result.Items,
            Search = Normalize(search),
            Page = page,
            TotalCount = result.TotalCount,
        });
    }

    [HttpGet("Create")]
    public IActionResult Create() => View(new IntrantFormViewModel());

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        IntrantFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await intrants.CreateIntrantAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(model);
        }

        TempData["Success"] = "Intrant enregistré avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var intrant = await intrants.GetIntrantByIdAsync(id, cancellationToken);
        if (intrant is null)
            return NotFound();

        return View(ToFormViewModel(intrant));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        IntrantFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.Id = id;

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await intrants.UpdateIntrantAsync(id, ToUpdateDto(model), cancellationToken);
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

        TempData["Success"] = "Intrant modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await intrants.DeleteIntrantAsync(id, cancellationToken);
            TempData["Success"] = "Intrant supprimé avec succès.";
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

    private static IntrantFormViewModel ToFormViewModel(IntrantDto intrant) =>
        new()
        {
            Id = intrant.Id,
            Nom = intrant.Nom,
            Unite = intrant.Unite,
        };

    private static CreateIntrantDto ToCreateDto(IntrantFormViewModel model) =>
        new(model.Nom.Trim(), model.Unite.Trim());

    private static UpdateIntrantDto ToUpdateDto(IntrantFormViewModel model) =>
        new(model.Nom.Trim(), model.Unite.Trim());

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
