using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.Services.Achat;
using OliveMorocco.Web.Models.Achat.BonsCommande;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/BonsCommande")]
public sealed class BonsCommandeAchatController(
    IBonCommandeFournisseurService bonsCommande,
    IFournisseurService clients) : Controller
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
            BonCommandeFournisseurListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new BonCommandeFournisseurListViewModel
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

        return View("Edit", new BonCommandeFournisseurFormViewModel
        {
            Numero = numero,
            Date = DateTime.Today,
        });
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        BonCommandeFournisseurFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ResolveFournisseurNomAsync(model, cancellationToken);
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
            await ResolveFournisseurNomAsync(model, cancellationToken);
            return View("Edit", model);
        }

        TempData["Success"] = "Bon de commande enregistr├⌐ avec succ├¿s.";
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
        BonCommandeFournisseurFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;
        await ResolveFournisseurNomAsync(model, cancellationToken);
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
            await ResolveFournisseurNomAsync(model, cancellationToken);
            return View(model);
        }

        TempData["Success"] = "Bon de commande modifi├⌐ avec succ├¿s.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await bonsCommande.DeleteBonCommandeAsync(id, cancellationToken);
            TempData["Success"] = "Bon de commande supprim├⌐ avec succ├¿s.";
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

    private async Task ResolveFournisseurNomAsync(BonCommandeFournisseurFormViewModel model, CancellationToken cancellationToken)
    {
        if (model.FournisseurId <= 0)
            return;

        var client = await clients.GetFournisseurByIdAsync(model.FournisseurId, cancellationToken);
        model.FournisseurNom = client?.Nom ?? string.Empty;
    }

    private async Task<BonCommandeFournisseurFormViewModel> ToFormViewModelAsync(
        BonCommandeFournisseurDto dto,
        CancellationToken cancellationToken)
    {
        var client = await clients.GetFournisseurByIdAsync(dto.FournisseurId, cancellationToken);

        return new BonCommandeFournisseurFormViewModel
        {
            Id = dto.Id,
            Numero = dto.Numero,
            FournisseurId = dto.FournisseurId,
            FournisseurNom = client?.Nom ?? string.Empty,
            Date = dto.Date.Date,
            Note = dto.Note,
            Lignes = dto.Lignes.Select(l => new BonCommandeFournisseurLigneViewModel
            {
                ProduitId = l.ProduitId ?? 0,
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

    private static CreateBonCommandeFournisseurDto ToCreateDto(BonCommandeFournisseurFormViewModel model) =>
        new(model.Numero.Trim(), model.FournisseurId,
            model.Date.Date,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static UpdateBonCommandeFournisseurDto ToUpdateDto(BonCommandeFournisseurFormViewModel model) =>
        new(model.FournisseurId,
            model.Date.Date,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static List<CreateBonCommandeFournisseurLigneDto> ToLineDtos(IEnumerable<BonCommandeFournisseurLigneViewModel> lignes) =>
        lignes
            .Select(l => new CreateBonCommandeFournisseurLigneDto(
                l.ProduitId > 0 ? l.ProduitId : null,
                null,
                l.Designation.Trim(),
                l.QuantiteCommandee,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA,
                Normalize(l.Unite)))
            .ToList();

    private void ValidateLignes(BonCommandeFournisseurFormViewModel model)
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

            if (line.QuantiteCommandee <= 0)
                ModelState.AddModelError($"{prefix}.QuantiteCommandee", "La quantit├⌐ doit ├¬tre positive.");

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
