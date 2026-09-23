using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Business.Services.Achat;

public interface IArticleSuggestionService
{
    Task<IReadOnlyList<ArticleSuggestionDto>> SearchArticlesAsync(
        string? search,
        CancellationToken cancellationToken = default);
}
