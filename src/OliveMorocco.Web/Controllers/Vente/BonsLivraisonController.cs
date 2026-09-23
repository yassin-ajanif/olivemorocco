using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Services.Vente;
using OliveMorocco.Web.Models.Vente.BonsLivraison;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/[controller]")]
public sealed class BonsLivraisonController(
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

        var result = await bonsLivraison.GetBonsLivraisonAsync(
            search,
            page,
            BonLivraisonListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new BonLivraisonListViewModel
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
        var numero = await bonsLivraison.GenerateNumeroAsync(cancellationToken);

        return View("Edit", new BonLivraisonFormViewModel
        {
            Numero = numero,
            Date = DateTime.Today,
        });
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        BonLivraisonFormViewModel model,
        CancellationToken cancellationToken)
    {
        await ResolveClientNomAsync(model, cancellationToken);
        ValidateLignes(model);

        if (!ModelState.IsValid)
            return View("Edit", model);

        try
        {
            await bonsLivraison.CreateBonLivraisonAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await ResolveClientNomAsync(model, cancellationToken);
            return View("Edit", model);
        }

        TempData["Success"] = "Bon de livraison enregistré avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var existing = await bonsLivraison.GetBonLivraisonByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        return View(await ToFormViewModelAsync(existing, cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        BonLivraisonFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Id = id;
        await ResolveClientNomAsync(model, cancellationToken);
        ValidateLignes(model);

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await bonsLivraison.UpdateBonLivraisonAsync(id, ToUpdateDto(model), cancellationToken);
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

        TempData["Success"] = "Bon de livraison modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await bonsLivraison.DeleteBonLivraisonAsync(id, cancellationToken);
            TempData["Success"] = "Bon de livraison supprimé avec succès.";
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

    private async Task ResolveClientNomAsync(BonLivraisonFormViewModel model, CancellationToken cancellationToken)
    {
        if (model.ClientId <= 0)
            return;

        var client = await clients.GetClientByIdAsync(model.ClientId, cancellationToken);
        model.ClientNom = client?.Nom ?? string.Empty;
    }

    private async Task<BonLivraisonFormViewModel> ToFormViewModelAsync(
        BonLivraisonClientDto dto,
        CancellationToken cancellationToken)
    {
        var client = await clients.GetClientByIdAsync(dto.ClientId, cancellationToken);

        return new BonLivraisonFormViewModel
        {
            Id = dto.Id,
            Numero = dto.Numero,
            ClientId = dto.ClientId,
            ClientNom = client?.Nom ?? string.Empty,
            Date = dto.Date.Date,
            Note = dto.Note,
            Lignes = dto.Lignes.Select(l => new BonLivraisonLigneViewModel
            {
                ProduitId = l.ProduitId,
                Reference = l.Reference,
                Designation = l.Designation,
                QuantiteCommandee = l.QuantiteCommandee,
                QuantiteLivree = l.QuantiteLivree,
                PrixUnitaireHT = l.PrixUnitaireHT,
                Remise = l.Remise,
                TauxTVA = l.TauxTVA,
            }).ToList(),
        };
    }

    private static CreateBonLivraisonClientDto ToCreateDto(BonLivraisonFormViewModel model) =>
        new(
            model.Numero.Trim(),
            model.ClientId,
            null,
            null,
            model.Date.Date,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static UpdateBonLivraisonClientDto ToUpdateDto(BonLivraisonFormViewModel model) =>
        new(
            model.ClientId,
            null,
            null,
            model.Date.Date,
            Normalize(model.Note) ?? string.Empty,
            ToLineDtos(model.Lignes));

    private static List<CreateBonLivraisonClientLigneDto> ToLineDtos(IEnumerable<BonLivraisonLigneViewModel> lignes) =>
        lignes
            .Select(l => new CreateBonLivraisonClientLigneDto(
                l.ProduitId,
                l.Designation.Trim(),
                l.QuantiteCommandee,
                l.QuantiteLivree,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA))
            .ToList();

    private void ValidateLignes(BonLivraisonFormViewModel model)
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

            if (line.QuantiteCommandee <= 0)
                ModelState.AddModelError($"{prefix}.QuantiteCommandee", "La quantité commandée doit être positive.");

            if (line.QuantiteLivree <= 0)
                ModelState.AddModelError($"{prefix}.QuantiteLivree", "La quantité livrée doit être positive.");

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
