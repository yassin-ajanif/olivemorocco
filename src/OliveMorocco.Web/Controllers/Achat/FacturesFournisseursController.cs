using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.Services.Achat;
using OliveMorocco.Web.Models.Achat.FacturesFournisseurs;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class FacturesFournisseursController(
    IFactureFournisseurService factures,
    IBonReceptionService bonsReception,
    IFournisseurService clients) : Controller
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
            FactureFournisseurListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new FactureFournisseurListViewModel
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

        return View("Edit", new FactureFournisseurFormViewModel
        {
            Numero = numero,
            Date = DateTime.Today,
            DateEcheance = DateTime.Today.AddDays(30),
        });
    }

    /// <summary>Prefills a new facture from one or more bons de rÃ©ception.</summary>
    [HttpGet("FromBonsReception")]
    public async Task<IActionResult> FromBonsReception(
        [FromQuery] int[] ids,
        CancellationToken cancellationToken)
    {
        var distinctIds = ids.Where(id => id > 0).Distinct().ToList();
        if (distinctIds.Count == 0)
        {
            TempData["Error"] = "Sélectionnez au moins un bon de réception.";
            return RedirectToAction("Index", "BonsReception");
        }

        var loaded = new List<BonReceptionDto>();
        foreach (var id in distinctIds)
        {
            var bl = await bonsReception.GetBonReceptionByIdAsync(id, cancellationToken);
            if (bl is null)
            {
                TempData["Error"] = $"Bon de rÃ©ception {id} introuvable.";
                return RedirectToAction("Index", "BonsReception");
            }

            loaded.Add(bl);
        }

        var clientIds = loaded.Select(b => b.FournisseurId).Distinct().ToList();
        if (clientIds.Count > 1)
        {
            TempData["Error"] = "Les bons de réception sélectionnés doivent appartenir au même fournisseur.";
            return RedirectToAction("Index", "BonsReception");
        }

        var alreadyInvoiced = loaded.FirstOrDefault(b => b.FactureFournisseurId is not null);
        if (alreadyInvoiced is not null)
        {
            TempData["Error"] =
                $"Le bon {alreadyInvoiced.Numero} est déjà lié à la facture {alreadyInvoiced.FactureNumero ?? alreadyInvoiced.FactureFournisseurId.ToString()}.";
            if (alreadyInvoiced.FactureFournisseurId is int factureId)
                return RedirectToAction(nameof(Edit), new { id = factureId });
            return RedirectToAction("Index", "BonsReception");
        }

        var empty = loaded.FirstOrDefault(b => b.Lignes.Count == 0);
        if (empty is not null)
        {
            TempData["Error"] = $"Le bon {empty.Numero} ne contient aucune ligne ├á facturer.";
            return RedirectToAction("Edit", "BonsReception", new { id = empty.Id });
        }

        var clientId = clientIds[0];
        var client = await clients.GetFournisseurByIdAsync(clientId, cancellationToken);
        var numero = await factures.GenerateNumeroAsync(cancellationToken);
        var blNumeros = string.Join(", ", loaded.Select(b => b.Numero));

        var model = new FactureFournisseurFormViewModel
        {
            Numero = numero,
            FournisseurId = clientId,
            FournisseurNom = client?.Nom ?? string.Empty,
            Date = DateTime.Today,
            DateEcheance = DateTime.Today.AddDays(30),
            Note = $"Depuis {blNumeros}",
            LockFournisseur = true,
            LinkedBonsReception = loaded
                .Select(b => new LinkedBonReceptionViewModel { Id = b.Id, Numero = b.Numero })
                .ToList(),
            Lignes = loaded
                .SelectMany(bl => bl.Lignes.Select(l => new FactureFournisseurLigneViewModel
                {
                    BonReceptionId = bl.Id,
                    ProduitId = l.ProduitId,
                    Reference = l.Reference,
                    Designation = l.Designation,
                    Unite = "U",
                    Quantite = l.QuantiteRecue,
                    PrixUnitaireHT = l.PrixUnitaireHT,
                    Remise = 0,
                    TauxTVA = l.TauxTVA,
                }))
                .ToList(),
        };

        return View("Edit", model);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        FactureFournisseurFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ResolveFournisseurNomAsync(model, cancellationToken);
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
            await ResolveFournisseurNomAsync(model, cancellationToken);
            await PopulateLinkedBonsReceptionAsync(model, cancellationToken);
            return View("Edit", model);
        }

        TempData["Success"] = "Facture enregistr├⌐e avec succ├¿s.";
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
        FactureFournisseurFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;
        await ResolveFournisseurNomAsync(model, cancellationToken);
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
            await ResolveFournisseurNomAsync(model, cancellationToken);
            await PopulateLinkedBonsReceptionAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Facture modifi├⌐e avec succ├¿s.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await factures.DeleteFactureAsync(id, cancellationToken);
            TempData["Success"] = "Facture supprim├⌐e avec succ├¿s.";
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

    private async Task ResolveFournisseurNomAsync(FactureFournisseurFormViewModel model, CancellationToken cancellationToken)
    {
        if (model.FournisseurId <= 0)
            return;

        var client = await clients.GetFournisseurByIdAsync(model.FournisseurId, cancellationToken);
        model.FournisseurNom = client?.Nom ?? string.Empty;
    }

    private async Task<FactureFournisseurFormViewModel> ToFormViewModelAsync(
        FactureFournisseurDto dto,
        CancellationToken cancellationToken)
    {
        var client = await clients.GetFournisseurByIdAsync(dto.FournisseurId, cancellationToken);

        var model = new FactureFournisseurFormViewModel
        {
            Id = dto.Id,
            Numero = dto.Numero,
            FournisseurId = dto.FournisseurId,
            FournisseurNom = client?.Nom ?? string.Empty,
            Date = dto.Date.Date,
            DateEcheance = dto.DateEcheance.Date,
            RemiseGlobale = dto.RemiseGlobale,
            EstPayee = dto.EstPayee,
            Note = dto.Note,
            Lignes = dto.Lignes.Select(l => new FactureFournisseurLigneViewModel
            {
                BonReceptionId = l.BonReceptionId,
                ProduitId = l.ProduitId ?? 0,
                Reference = l.Reference,
                Designation = l.Designation,
                Quantite = l.Quantite,
                Unite = l.Conditionnement,
                PrixUnitaireHT = l.PrixUnitaireHT,
                Remise = l.Remise,
                TauxTVA = l.TauxTVA,
            }).ToList(),
        };

        await PopulateLinkedBonsReceptionAsync(model, cancellationToken);
        return model;
    }

    private async Task PopulateLinkedBonsReceptionAsync(
        FactureFournisseurFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.LinkedBonsReception.Count > 0)
            return;

        var ids = model.Lignes
            .Where(l => l.BonReceptionId is > 0)
            .Select(l => l.BonReceptionId!.Value)
            .Distinct()
            .ToList();

        foreach (var id in ids)
        {
            var bl = await bonsReception.GetBonReceptionByIdAsync(id, cancellationToken);
            if (bl is not null)
            {
                model.LinkedBonsReception.Add(new LinkedBonReceptionViewModel
                {
                    Id = bl.Id,
                    Numero = bl.Numero,
                });
            }
        }
    }

    private static CreateFactureFournisseurDto ToCreateDto(FactureFournisseurFormViewModel model) =>
        new(model.Numero.Trim(), model.FournisseurId,
            model.Date.Date,
            model.DateEcheance.Date,
            model.RemiseGlobale,
            0,
            model.EstPayee,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static UpdateFactureFournisseurDto ToUpdateDto(FactureFournisseurFormViewModel model) =>
        new(model.FournisseurId,
            model.Date.Date,
            model.DateEcheance.Date,
            model.RemiseGlobale,
            0,
            model.EstPayee,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static List<CreateFactureFournisseurLigneDto> ToLineDtos(IEnumerable<FactureFournisseurLigneViewModel> lignes) =>
        lignes
            .Select(l => new CreateFactureFournisseurLigneDto(
                l.BonReceptionId is > 0 ? l.BonReceptionId : null,
                l.ProduitId > 0 ? l.ProduitId : null,
                null,
                l.Designation.Trim(),
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA,
                Normalize(l.Unite)))
            .ToList();

    private void ValidateLignes(FactureFournisseurFormViewModel model)
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
                ModelState.AddModelError($"{prefix}.Designation", "La d├⌐signation est obligatoire.");

            if (string.IsNullOrWhiteSpace(line.Unite))
                ModelState.AddModelError($"{prefix}.Unite", "L'unit├⌐ est obligatoire.");

            if (line.ProduitId <= 0)
                ModelState.AddModelError($"{prefix}.Designation", "S├⌐lectionnez un article depuis la recherche.");

            if (line.Quantite <= 0)
                ModelState.AddModelError($"{prefix}.Quantite", "La quantit├⌐ doit ├¬tre positive.");

            if (line.PrixUnitaireHT < 0)
                ModelState.AddModelError($"{prefix}.PrixUnitaireHT", "Le prix unitaire doit ├¬tre positif ou nul.");
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
