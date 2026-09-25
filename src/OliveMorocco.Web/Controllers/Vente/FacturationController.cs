using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Services.Vente;
using OliveMorocco.Web.Models.Vente.Facturation;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/[controller]")]
public sealed class FacturationController(
    IFactureClientService factures,
    IBonLivraisonClientService bonsLivraison,
    IClientService clients) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await factures.GetFacturesAsync(
            search,
            page,
            FactureListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new FactureListViewModel
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
        var numero = await factures.GenerateNumeroAsync(cancellationToken);

        return View("Edit", new FactureFormViewModel
        {
            Numero = numero,
            Date = DateTime.Today,
            DateEcheance = DateTime.Today.AddDays(30),
        });
    }

    /// <summary>Prefills a new facture from one or more bons de livraison.</summary>
    [HttpGet("FromBonsLivraison")]
    public async Task<IActionResult> FromBonsLivraison(
        [FromQuery] int[] ids,
        CancellationToken cancellationToken)
    {
        var distinctIds = ids.Where(id => id > 0).Distinct().ToList();
        if (distinctIds.Count == 0)
        {
            TempData["Error"] = "Sélectionnez au moins un bon de livraison.";
            return RedirectToAction("Index", "BonsLivraison");
        }

        var loaded = new List<BonLivraisonClientDto>();
        foreach (var id in distinctIds)
        {
            var bl = await bonsLivraison.GetBonLivraisonByIdAsync(id, cancellationToken);
            if (bl is null)
            {
                TempData["Error"] = $"Bon de livraison {id} introuvable.";
                return RedirectToAction("Index", "BonsLivraison");
            }

            loaded.Add(bl);
        }

        var clientIds = loaded.Select(b => b.ClientId).Distinct().ToList();
        if (clientIds.Count > 1)
        {
            TempData["Error"] = "Les bons de livraison sélectionnés doivent appartenir au même client.";
            return RedirectToAction("Index", "BonsLivraison");
        }

        var alreadyInvoiced = loaded.FirstOrDefault(b => b.FactureId is not null);
        if (alreadyInvoiced is not null)
        {
            TempData["Error"] =
                $"Le bon {alreadyInvoiced.Numero} est déjà lié à la facture {alreadyInvoiced.FactureNumero ?? alreadyInvoiced.FactureId.ToString()}.";
            if (alreadyInvoiced.FactureId is int factureId)
                return RedirectToAction(nameof(Edit), new { id = factureId });
            return RedirectToAction("Index", "BonsLivraison");
        }

        var empty = loaded.FirstOrDefault(b => b.Lignes.Count == 0);
        if (empty is not null)
        {
            TempData["Error"] = $"Le bon {empty.Numero} ne contient aucune ligne à facturer.";
            return RedirectToAction("Edit", "BonsLivraison", new { id = empty.Id });
        }

        var clientId = clientIds[0];
        var client = await clients.GetClientByIdAsync(clientId, cancellationToken);
        var numero = await factures.GenerateNumeroAsync(cancellationToken);
        var blNumeros = string.Join(", ", loaded.Select(b => b.Numero));

        var model = new FactureFormViewModel
        {
            Numero = numero,
            ClientId = clientId,
            ClientNom = client?.Nom ?? string.Empty,
            Date = DateTime.Today,
            DateEcheance = DateTime.Today.AddDays(30),
            Note = $"Depuis {blNumeros}",
            LockClient = true,
            LinkedBonsLivraison = loaded
                .Select(b => new LinkedBonLivraisonViewModel { Id = b.Id, Numero = b.Numero })
                .ToList(),
            Lignes = loaded
                .SelectMany(bl => bl.Lignes.Select(l => new FactureLigneViewModel
                {
                    BonLivraisonId = bl.Id,
                    ProduitId = l.ProduitId,
                    Reference = l.Reference,
                    Designation = l.Designation,
                    Unite = "U",
                    Quantite = l.QuantiteLivree,
                    PrixUnitaireHT = l.PrixUnitaireHT,
                    Remise = l.Remise,
                    TauxTVA = l.TauxTVA,
                }))
                .ToList(),
        };

        return View("Edit", model);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        FactureFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ResolveClientNomAsync(model, cancellationToken);
        ValidateLignes(model);

        if (!ModelState.IsValid)
            return View("Edit", model);

        try
        {
            await factures.CreateFactureAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await ResolveClientNomAsync(model, cancellationToken);
            await PopulateLinkedBonsLivraisonAsync(model, cancellationToken);
            return View("Edit", model);
        }

        TempData["Success"] = "Facture enregistrée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var existing = await factures.GetFactureByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        return View(await ToFormViewModelAsync(existing, cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        FactureFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;
        await ResolveClientNomAsync(model, cancellationToken);
        ValidateLignes(model);

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await factures.UpdateFactureAsync(id, ToUpdateDto(model), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await ResolveClientNomAsync(model, cancellationToken);
            await PopulateLinkedBonsLivraisonAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Facture modifiée avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await factures.DeleteFactureAsync(id, cancellationToken);
            TempData["Success"] = "Facture supprimée avec succès.";
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

    private async Task ResolveClientNomAsync(FactureFormViewModel model, CancellationToken cancellationToken)
    {
        if (model.ClientId <= 0)
            return;

        var client = await clients.GetClientByIdAsync(model.ClientId, cancellationToken);
        model.ClientNom = client?.Nom ?? string.Empty;
    }

    private async Task<FactureFormViewModel> ToFormViewModelAsync(
        FactureClientDto dto,
        CancellationToken cancellationToken)
    {
        var client = await clients.GetClientByIdAsync(dto.ClientId, cancellationToken);

        var model = new FactureFormViewModel
        {
            Id = dto.Id,
            Numero = dto.Numero,
            ClientId = dto.ClientId,
            ClientNom = client?.Nom ?? string.Empty,
            Date = dto.Date.Date,
            DateEcheance = dto.DateEcheance.Date,
            BonCommandeReference = dto.BonCommandeReference,
            RemiseGlobale = dto.RemiseGlobale,
            EstPayee = dto.EstPayee,
            Note = dto.Note,
            Lignes = dto.Lignes.Select(l => new FactureLigneViewModel
            {
                BonLivraisonId = l.BonLivraisonId,
                ProduitId = l.ProduitId,
                Reference = l.Reference,
                Designation = l.Designation,
                Quantite = l.Quantite,
                Unite = l.Conditionnement,
                PrixUnitaireHT = l.PrixUnitaireHT,
                Remise = l.Remise,
                TauxTVA = l.TauxTVA,
            }).ToList(),
        };

        await PopulateLinkedBonsLivraisonAsync(model, cancellationToken);
        return model;
    }

    private async Task PopulateLinkedBonsLivraisonAsync(
        FactureFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.LinkedBonsLivraison.Count > 0)
            return;

        var ids = model.Lignes
            .Where(l => l.BonLivraisonId is > 0)
            .Select(l => l.BonLivraisonId!.Value)
            .Distinct()
            .ToList();

        foreach (var id in ids)
        {
            var bl = await bonsLivraison.GetBonLivraisonByIdAsync(id, cancellationToken);
            if (bl is not null)
            {
                model.LinkedBonsLivraison.Add(new LinkedBonLivraisonViewModel
                {
                    Id = bl.Id,
                    Numero = bl.Numero,
                });
            }
        }
    }

    private static CreateFactureClientDto ToCreateDto(FactureFormViewModel model) =>
        new(
            model.Numero.Trim(),
            model.ClientId,
            null,
            model.Date.Date,
            model.DateEcheance.Date,
            Normalize(model.BonCommandeReference) ?? string.Empty,
            model.RemiseGlobale,
            0,
            model.EstPayee,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static UpdateFactureClientDto ToUpdateDto(FactureFormViewModel model) =>
        new(
            model.ClientId,
            null,
            model.Date.Date,
            model.DateEcheance.Date,
            Normalize(model.BonCommandeReference) ?? string.Empty,
            model.RemiseGlobale,
            0,
            model.EstPayee,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static List<CreateFactureClientLigneDto> ToLineDtos(IEnumerable<FactureLigneViewModel> lignes) =>
        lignes
            .Select(l => new CreateFactureClientLigneDto(
                l.BonLivraisonId is > 0 ? l.BonLivraisonId : null,
                l.ProduitId,
                l.Designation.Trim(),
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA,
                Normalize(l.Unite)))
            .ToList();

    private void ValidateLignes(FactureFormViewModel model)
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

            if (line.Quantite <= 0)
                ModelState.AddModelError($"{prefix}.Quantite", "La quantité doit être positive.");

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
