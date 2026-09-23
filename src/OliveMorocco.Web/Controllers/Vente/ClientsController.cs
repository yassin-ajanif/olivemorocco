using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Services.Vente;
using OliveMorocco.Web.Models.Vente.Clients;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/[controller]")]
public sealed class ClientsController(IClientService clients) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await clients.GetClientsAsync(
            search,
            page,
            ClientListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new ClientListViewModel
        {
            Items = result.Items,
            Search = Normalize(search),
            Page = page,
            TotalCount = result.TotalCount,
        });
    }

    [HttpGet("Create")]
    public IActionResult Create() => View(new ClientFormViewModel());

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ClientFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await clients.CreateClientAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(model);
        }

        TempData["Success"] = "Client enregistré avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var client = await clients.GetClientByIdAsync(id, cancellationToken);
        if (client is null)
            return NotFound();

        return View(ToFormViewModel(client));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ClientFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.Id = id;

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await clients.UpdateClientAsync(id, ToUpdateDto(model), cancellationToken);
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

        TempData["Success"] = "Client modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await clients.DeleteClientAsync(id, cancellationToken);
            TempData["Success"] = "Client supprimé avec succès.";
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

    private static ClientFormViewModel ToFormViewModel(ClientDto client) =>
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

    private static CreateClientDto ToCreateDto(ClientFormViewModel model) =>
        new(
            model.Nom.Trim(),
            TrimOrEmpty(model.Adresse),
            TrimOrEmpty(model.Ville),
            TrimOrEmpty(model.Telephone),
            TrimOrEmpty(model.Email),
            TrimOrEmpty(model.ICE),
            TrimOrEmpty(model.ConditionsPaiement),
            model.Actif);

    private static UpdateClientDto ToUpdateDto(ClientFormViewModel model) =>
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
