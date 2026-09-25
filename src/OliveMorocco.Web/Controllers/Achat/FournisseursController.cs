using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.Services.Achat;
using OliveMorocco.Web.Models.Achat.Fournisseurs;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class FournisseursController(IFournisseurService clients) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await clients.GetFournisseursAsync(
            search,
            page,
            FournisseurListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new FournisseurListViewModel
        {
            Items = result.Items,
            Search = Normalize(search),
            Page = page,
            TotalCount = result.TotalCount,
        });
    }

    [HttpGet("Create")]
    public IActionResult Create() => View(new FournisseurFormViewModel());

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        FournisseurFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await clients.CreateFournisseurAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(model);
        }

        TempData["Success"] = "Fournisseur enregistré avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var client = await clients.GetFournisseurByIdAsync(id, cancellationToken);
        if (client is null)
            return NotFound();

        return View(ToFormViewModel(client));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        FournisseurFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.Id = id;

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await clients.UpdateFournisseurAsync(id, ToUpdateDto(model), cancellationToken);
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

        TempData["Success"] = "Fournisseur modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await clients.DeleteFournisseurAsync(id, cancellationToken);
            TempData["Success"] = "Fournisseur supprimé avec succès.";
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
            var client = await clients.ToggleActifAsync(id, cancellationToken);
            TempData["Success"] = client.Actif
                ? $"{client.Nom} a été activé."
                : $"{client.Nom} a été désactivé.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index), new { search, page });
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    private static FournisseurFormViewModel ToFormViewModel(FournisseurDto client) =>
        new()
        {
            Id = client.Id,
            Nom = client.Nom,
            ICE = client.ICE,
            Adresse = client.Adresse,
            Ville = client.Ville,
            Telephone = client.Telephone,
            Email = client.Email,
            ConditionsPaiement = client.ConditionsPaiement,
            Actif = client.Actif,
        };

    private static CreateFournisseurDto ToCreateDto(FournisseurFormViewModel model) =>
        new(
            model.Nom.Trim(),
            TrimOrEmpty(model.Adresse),
            TrimOrEmpty(model.Ville),
            TrimOrEmpty(model.Telephone),
            TrimOrEmpty(model.Email),
            TrimOrEmpty(model.ICE),
            TrimOrEmpty(model.ConditionsPaiement),
            model.Actif);

    private static UpdateFournisseurDto ToUpdateDto(FournisseurFormViewModel model) =>
        new(
            model.Nom.Trim(),
            TrimOrEmpty(model.Adresse),
            TrimOrEmpty(model.Ville),
            TrimOrEmpty(model.Telephone),
            TrimOrEmpty(model.Email),
            TrimOrEmpty(model.ICE),
            TrimOrEmpty(model.ConditionsPaiement),
            model.Actif);

    private static string TrimOrEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
