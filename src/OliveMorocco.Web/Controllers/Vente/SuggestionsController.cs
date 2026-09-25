using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Business.Services.Vente;
using OliveMorocco.Web.Models.Vente.Devis;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/[controller]")]
public sealed class SuggestionsController(
    IClientService clients,
    IArticleSuggestionService articles) : Controller
{
    private const int SuggestionPageSize = 15;

    [HttpGet("Clients")]
    public async Task<IActionResult> Clients(string? search, CancellationToken cancellationToken)
    {
        var result = await clients.GetClientsAsync(
            Normalize(search),
            page: 1,
            pageSize: SuggestionPageSize,
            cancellationToken: cancellationToken);

        var items = result.Items
            .Select(c => new ClientSelectItem { Id = c.Id, Nom = c.Nom })
            .ToList();

        return PartialView("~/Views/Vente/Devis/_ClientSuggestions.cshtml", items);
    }

    [HttpGet("Articles")]
    public async Task<IActionResult> Articles(string? search, CancellationToken cancellationToken)
    {
        var items = await articles.SearchArticlesAsync(Normalize(search), cancellationToken);
        return PartialView("~/Views/Vente/Devis/_ArticleSuggestions.cshtml", items);
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
