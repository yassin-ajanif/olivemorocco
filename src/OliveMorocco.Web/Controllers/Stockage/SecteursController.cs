using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Business.Services.Stockage;
using OliveMorocco.Web.Models.Stockage.Secteurs;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class SecteursController(
    ISecteurService secteurs,
    IVarieteService varietes) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);

        var result = await secteurs.GetSecteursAsync(
            search,
            page,
            SecteurListViewModel.DefaultPageSize,
            cancellationToken);

        return View(new SecteurListViewModel
        {
            Items = result.Items,
            Search = Normalize(search),
            Page = page,
            TotalCount = result.TotalCount,
        });
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        => View(await BuildFormAsync(cancellationToken: cancellationToken));

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        SecteurFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await secteurs.CreateSecteurAsync(ToCreateDto(model), cancellationToken);
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        TempData["Success"] = "Secteur enregistré avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var secteur = await secteurs.GetSecteurByIdAsync(id, cancellationToken);
        if (secteur is null)
            return NotFound();

        return View(await BuildFormAsync(ToFormViewModel(secteur), cancellationToken));
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        SecteurFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        model.Id = id;

        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model, cancellationToken));

        try
        {
            await secteurs.UpdateSecteurAsync(id, ToUpdateDto(model), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(await BuildFormAsync(model, cancellationToken));
        }

        TempData["Success"] = "Secteur modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await secteurs.DeleteSecteurAsync(id, cancellationToken);
            TempData["Success"] = "Secteur supprimé avec succès.";
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

    [HttpPost("Varietes")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVariete(
        [FromForm] string nom,
        [FromForm] string? code,
        [FromForm] string? regionOrigine,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = new CreateVarieteDto(
                nom?.Trim() ?? string.Empty,
                NormalizeOptional(code),
                NormalizeOptional(regionOrigine));

            var created = await varietes.CreateVarieteAsync(dto, cancellationToken);
            return Json(new { id = created.Id, nom = created.Nom });
        }
        catch (ValidationException exception)
        {
            return BadRequest(new
            {
                error = exception.Errors.FirstOrDefault()?.ErrorMessage ?? "Données invalides.",
            });
        }
    }

    private async Task<SecteurFormViewModel> BuildFormAsync(
        SecteurFormViewModel? model = null,
        CancellationToken cancellationToken = default)
    {
        var varieteList = await secteurs.GetVarietesForSelectAsync(cancellationToken);

        if (model is null)
        {
            return new SecteurFormViewModel
            {
                Varietes = varieteList,
            };
        }

        model.Varietes = varieteList;
        return model;
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    private static SecteurFormViewModel ToFormViewModel(SecteurDto secteur) =>
        new()
        {
            Id = secteur.Id,
            Nom = secteur.Nom,
            Code = secteur.Code,
            SuperficieHectares = secteur.SuperficieHectares,
            Lignes = secteur.Lignes
                .Select(l => new SecteurVarieteLineViewModel
                {
                    VarieteId = l.VarieteId,
                    SuperficieHectares = l.SuperficieHectares,
                })
                .ToList(),
        };

    private static CreateSecteurDto ToCreateDto(SecteurFormViewModel model) =>
        new(
            model.Nom.Trim(),
            NormalizeOptional(model.Code),
            model.SuperficieHectares,
            ToLineDtos(model.Lignes));

    private static UpdateSecteurDto ToUpdateDto(SecteurFormViewModel model) =>
        new(
            model.Nom.Trim(),
            NormalizeOptional(model.Code),
            model.SuperficieHectares,
            ToLineDtos(model.Lignes));

    private static IReadOnlyList<CreateSecteurVarieteLineDto> ToLineDtos(
        IEnumerable<SecteurVarieteLineViewModel> lignes) =>
        lignes
            .Where(l => l.VarieteId > 0 && l.SuperficieHectares > 0)
            .Select(l => new CreateSecteurVarieteLineDto(l.VarieteId, l.SuperficieHectares))
            .ToList();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
