using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Business.Services.Achat;

public interface IIntrantSuggestionService
{
    Task<IReadOnlyList<IntrantSuggestionDto>> SearchIntrantsAsync(
        string? search,
        CancellationToken cancellationToken = default);
}
