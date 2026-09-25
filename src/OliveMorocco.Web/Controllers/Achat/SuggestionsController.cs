using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.Services.Achat;
using OliveMorocco.Web.Models.Achat;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class SuggestionsController(
    IFournisseurService clients,
    IIntrantSuggestionService intrants) : Controller
{
    private const int SuggestionPageSize = 15;

    [HttpGet("Fournisseurs")]
    public async Task<IActionResult> Fournisseurs(string? search, CancellationToken cancellationToken)
    {
        var result = await clients.GetFournisseursAsync(
            Normalize(search),
            page: 1,
            pageSize: SuggestionPageSize,
            cancellationToken: cancellationToken);

        var items = result.Items
            .Select(c => new FournisseurSelectItem { Id = c.Id, Nom = c.Nom })
            .ToList();

        return PartialView("~/Views/Achat/Suggestions/_FournisseurSuggestions.cshtml", items);
    }

    [HttpGet("Intrants")]
    public async Task<IActionResult> Intrants(string? search, CancellationToken cancellationToken)
    {
        var items = await intrants.SearchIntrantsAsync(Normalize(search), cancellationToken);
        return PartialView("~/Views/Achat/Suggestions/_IntrantSuggestions.cshtml", items);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
