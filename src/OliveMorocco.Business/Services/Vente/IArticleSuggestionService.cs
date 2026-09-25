using OliveMorocco.Business.DTOs.Vente;

namespace OliveMorocco.Business.Services.Vente;

public interface IArticleSuggestionService
{
    Task<IReadOnlyList<ArticleSuggestionDto>> SearchArticlesAsync(
        string? search,
        CancellationToken cancellationToken = default);
}
