using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.Services.Achat;
using OliveMorocco.Web.Models.Achat;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class SuggestionsController(
    IFournisseurService clients,
    IArticleSuggestionService articles) : Controller
{
    private const int SuggestionPageSize = 15;

    [HttpGet("Clients")]
    public async Task<IActionResult> Clients(string? search, CancellationToken cancellationToken)
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

    [HttpGet("Articles")]
    public async Task<IActionResult> Articles(string? search, CancellationToken cancellationToken)
    {
        var items = await articles.SearchArticlesAsync(Normalize(search), cancellationToken);
        return PartialView("~/Views/Achat/Suggestions/_ArticleSuggestions.cshtml", items);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
