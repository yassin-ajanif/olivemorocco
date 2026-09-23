using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Services.Vente;
using OliveMorocco.Web.Models.Vente.BonsCommande;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/BonsCommande")]
public sealed class BonsCommandeController(
    IBonCommandeClientService bonsCommande,
    IClientService clients) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await bonsCommande.GetBonsCommandeAsync(
            search,
            page,
            BonCommandeListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new BonCommandeListViewModel
        {
            Items = result.Items,
            Search = Normalize(search),
            Page = page,
            TotalCount = result.TotalCount,
        });
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var numero = await bonsCommande.GenerateNumeroAsync(cancellationToken);

        return View("Edit", new BonCommandeFormViewModel
        {
            Numero = numero,
            Date = DateTime.Today,
        });
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        BonCommandeFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ResolveClientNomAsync(model, cancellationToken);
        ValidateLignes(model);

        if (!ModelState.IsValid)
            return View("Edit", model);

        try
        {
            await bonsCommande.CreateBonCommandeAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await ResolveClientNomAsync(model, cancellationToken);
            return View("Edit", model);
        }

        TempData["Success"] = "Bon de commande enregistré avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var existing = await bonsCommande.GetBonCommandeByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        return View(await ToFormViewModelAsync(existing, cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        BonCommandeFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;
        await ResolveClientNomAsync(model, cancellationToken);
        ValidateLignes(model);

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await bonsCommande.UpdateBonCommandeAsync(id, ToUpdateDto(model), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await ResolveClientNomAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Bon de commande modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await bonsCommande.DeleteBonCommandeAsync(id, cancellationToken);
            TempData["Success"] = "Bon de commande supprimé avec succès.";
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

    private async Task ResolveClientNomAsync(BonCommandeFormViewModel model, CancellationToken cancellationToken)
    {
        if (model.ClientId <= 0)
            return;

        var client = await clients.GetClientByIdAsync(model.ClientId, cancellationToken);
        model.ClientNom = client?.Nom ?? string.Empty;
    }

    private async Task<BonCommandeFormViewModel> ToFormViewModelAsync(
        BonCommandeClientDto dto,
        CancellationToken cancellationToken)
    {
        var client = await clients.GetClientByIdAsync(dto.ClientId, cancellationToken);

        return new BonCommandeFormViewModel
        {
            Id = dto.Id,
            Numero = dto.Numero,
            ClientId = dto.ClientId,
            ClientNom = client?.Nom ?? string.Empty,
            Date = dto.Date.Date,
            Note = dto.Note,
            Lignes = dto.Lignes.Select(l => new BonCommandeLigneViewModel
            {
                ProduitId = l.ProduitId,
                Reference = l.Reference,
                Designation = l.Designation,
                QuantiteCommandee = l.QuantiteCommandee,
                Unite = l.Conditionnement,
                PrixUnitaireHT = l.PrixUnitaireHT,
                Remise = l.Remise,
                TauxTVA = l.TauxTVA,
            }).ToList(),
        };
    }

    private static CreateBonCommandeClientDto ToCreateDto(BonCommandeFormViewModel model) =>
        new(
            model.Numero.Trim(),
            model.ClientId,
            null,
            model.Date.Date,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static UpdateBonCommandeClientDto ToUpdateDto(BonCommandeFormViewModel model) =>
        new(
            model.ClientId,
            null,
            model.Date.Date,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static List<CreateBonCommandeClientLigneDto> ToLineDtos(IEnumerable<BonCommandeLigneViewModel> lignes) =>
        lignes
            .Select(l => new CreateBonCommandeClientLigneDto(
                l.ProduitId,
                l.Designation.Trim(),
                l.QuantiteCommandee,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA,
                Normalize(l.Unite)))
            .ToList();

    private void ValidateLignes(BonCommandeFormViewModel model)
    {
        if (model.Lignes.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Ajoutez au moins une ligne via la recherche d'articles.");
            return;
        }

        for (var i = 0; i < model.Lignes.Count; i++)
        {
            var line = model.Lignes[i];
            var prefix = $"Lignes[{i}]";

            if (string.IsNullOrWhiteSpace(line.Designation))
                ModelState.AddModelError($"{prefix}.Designation", "La désignation est obligatoire.");

            if (string.IsNullOrWhiteSpace(line.Unite))
                ModelState.AddModelError($"{prefix}.Unite", "L'unité est obligatoire.");

            if (line.ProduitId <= 0)
                ModelState.AddModelError($"{prefix}.Designation", "Sélectionnez un article depuis la recherche.");

            if (line.QuantiteCommandee <= 0)
                ModelState.AddModelError($"{prefix}.QuantiteCommandee", "La quantité doit être positive.");

            if (line.PrixUnitaireHT < 0)
                ModelState.AddModelError($"{prefix}.PrixUnitaireHT", "Le prix unitaire doit être positif ou nul.");
        }
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
