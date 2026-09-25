using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.Services.Achat;
using OliveMorocco.Web.Models.Achat.BonsReception;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class BonsReceptionController(
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

        var result = await bonsReception.GetBonsReceptionAsync(
            search,
            page,
            BonReceptionListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new BonReceptionListViewModel
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
        var numero = await bonsReception.GenerateNumeroAsync(cancellationToken);

        return View("Edit", new BonReceptionFormViewModel
        {
            Numero = numero,
            Date = DateTime.Today,
        });
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        BonReceptionFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ResolveFournisseurNomAsync(model, cancellationToken);
        ValidateLignes(model);

        if (!ModelState.IsValid)
            return View("Edit", model);

        try
        {
            await bonsReception.CreateBonReceptionAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await ResolveFournisseurNomAsync(model, cancellationToken);
            return View("Edit", model);
        }

        TempData["Success"] = "Bon de réception enregistré avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var existing = await bonsReception.GetBonReceptionByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        return View(await ToFormViewModelAsync(existing, cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        BonReceptionFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;
        await ResolveFournisseurNomAsync(model, cancellationToken);
        ValidateLignes(model);

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await bonsReception.UpdateBonReceptionAsync(id, ToUpdateDto(model), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await ResolveFournisseurNomAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Bon de réception modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await bonsReception.DeleteBonReceptionAsync(id, cancellationToken);
            TempData["Success"] = "Bon de réception supprimé avec succès.";
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

    private async Task ResolveFournisseurNomAsync(BonReceptionFormViewModel model, CancellationToken cancellationToken)
    {
        if (model.FournisseurId <= 0)
            return;

        var client = await clients.GetFournisseurByIdAsync(model.FournisseurId, cancellationToken);
        model.FournisseurNom = client?.Nom ?? string.Empty;
    }

    private async Task<BonReceptionFormViewModel> ToFormViewModelAsync(
        BonReceptionDto dto,
        CancellationToken cancellationToken)
    {
        var client = await clients.GetFournisseurByIdAsync(dto.FournisseurId, cancellationToken);

        return new BonReceptionFormViewModel
        {
            Id = dto.Id,
            Numero = dto.Numero,
            FournisseurId = dto.FournisseurId,
            FournisseurNom = client?.Nom ?? string.Empty,
            Date = dto.Date.Date,
            Note = dto.Note,
            FactureId = dto.FactureFournisseurId,
            FactureNumero = dto.FactureNumero,
            Lignes = dto.Lignes.Select(l => new BonReceptionLigneViewModel
            {
                ProduitId = l.ProduitId,
                Reference = l.Reference,
                Designation = l.Designation,
                QuantiteRecue = l.QuantiteRecue,
                PrixUnitaireHT = l.PrixUnitaireHT,
                Remise = 0,
                TauxTVA = l.TauxTVA,
            }).ToList(),
        };
    }

    private static CreateBonReceptionDto ToCreateDto(BonReceptionFormViewModel model) =>
        new(model.Numero.Trim(), model.FournisseurId,
            null,
            model.Date.Date,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static UpdateBonReceptionDto ToUpdateDto(BonReceptionFormViewModel model) =>
        new(model.FournisseurId,
            null,
            model.Date.Date,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static List<CreateBonReceptionLigneDto> ToLineDtos(IEnumerable<BonReceptionLigneViewModel> lignes) =>
        lignes
            .Select(l => new CreateBonReceptionLigneDto(
                l.ProduitId,
                l.Designation.Trim(),
                l.QuantiteRecue,
                l.PrixUnitaireHT,
                l.TauxTVA))
            .ToList();

    private void ValidateLignes(BonReceptionFormViewModel model)
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

            if (line.ProduitId <= 0)
                ModelState.AddModelError($"{prefix}.Designation", "Sélectionnez un article depuis la recherche.");

            if (line.QuantiteRecue <= 0)
                ModelState.AddModelError($"{prefix}.QuantiteRecue", "La quantité reçue doit être positive.");

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
