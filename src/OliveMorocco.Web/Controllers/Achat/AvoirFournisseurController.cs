using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.Services.Achat;
using OliveMorocco.Web.Models.Achat.AvoirFournisseur;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class AvoirFournisseurController(
    IAvoirFournisseurService avoirs,
    IFournisseurService clients) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await avoirs.GetAvoirsAsync(
            search,
            page,
            AvoirFournisseurListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new AvoirFournisseurListViewModel
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
        var numero = await avoirs.GenerateNumeroAsync(cancellationToken);

        return View("Edit", new AvoirFournisseurFormViewModel
        {
            Numero = numero,
            Date = DateTime.Today,
        });
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        AvoirFournisseurFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ResolveFournisseurNomAsync(model, cancellationToken);
        ValidateLignes(model);

        if (!ModelState.IsValid)
            return View("Edit", model);

        try
        {
            await avoirs.CreateAvoirAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await ResolveFournisseurNomAsync(model, cancellationToken);
            return View("Edit", model);
        }

        TempData["Success"] = "Avoir enregistr├⌐ avec succ├¿s.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var existing = await avoirs.GetAvoirByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        return View(await ToFormViewModelAsync(existing, cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        AvoirFournisseurFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;
        await ResolveFournisseurNomAsync(model, cancellationToken);
        ValidateLignes(model);

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await avoirs.UpdateAvoirAsync(id, ToUpdateDto(model), cancellationToken);
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

        TempData["Success"] = "Avoir modifi├⌐ avec succ├¿s.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await avoirs.DeleteAvoirAsync(id, cancellationToken);
            TempData["Success"] = "Avoir supprim├⌐ avec succ├¿s.";
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

    private async Task ResolveFournisseurNomAsync(AvoirFournisseurFormViewModel model, CancellationToken cancellationToken)
    {
        if (model.FournisseurId <= 0)
            return;

        var client = await clients.GetFournisseurByIdAsync(model.FournisseurId, cancellationToken);
        model.FournisseurNom = client?.Nom ?? string.Empty;
    }

    private async Task<AvoirFournisseurFormViewModel> ToFormViewModelAsync(
        AvoirFournisseurDto dto,
        CancellationToken cancellationToken)
    {
        var client = await clients.GetFournisseurByIdAsync(dto.FournisseurId, cancellationToken);

        return new AvoirFournisseurFormViewModel
        {
            Id = dto.Id,
            Numero = dto.Numero,
            FournisseurId = dto.FournisseurId,
            FournisseurNom = client?.Nom ?? string.Empty,
            FactureId = dto.FactureFournisseurId,
            Date = dto.Date.Date,
            Motif = dto.Motif,
            RetourMarchandise = dto.RetourMarchandise,
            Lignes = dto.Lignes.Select(l => new AvoirFournisseurLigneViewModel
            {
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
    }

    private static CreateAvoirFournisseurDto ToCreateDto(AvoirFournisseurFormViewModel model) =>
        new(
            model.Numero.Trim(),
            model.FournisseurId,
            NormalizeFactureId(model.FactureId),
            model.Date.Date,
            Normalize(model.Motif) ?? string.Empty,
            model.RetourMarchandise,
            ToLineDtos(model.Lignes));

    private static UpdateAvoirFournisseurDto ToUpdateDto(AvoirFournisseurFormViewModel model) =>
        new(
            model.FournisseurId,
            NormalizeFactureId(model.FactureId),
            model.Date.Date,
            Normalize(model.Motif) ?? string.Empty,
            model.RetourMarchandise,
            ToLineDtos(model.Lignes));

    private static List<CreateAvoirFournisseurLigneDto> ToLineDtos(IEnumerable<AvoirFournisseurLigneViewModel> lignes) =>
        lignes
            .Select(l => new CreateAvoirFournisseurLigneDto(
                l.ProduitId,
                l.Designation.Trim(),
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA,
                Normalize(l.Unite)))
            .ToList();

    private void ValidateLignes(AvoirFournisseurFormViewModel model)
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

    private static int? NormalizeFactureId(int? factureId) =>
        factureId is > 0 ? factureId : null;
}
