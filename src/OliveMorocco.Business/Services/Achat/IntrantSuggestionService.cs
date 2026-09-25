using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Business.Services.Achat;

public sealed class IntrantSuggestionService(IRepository<Intrant> intrants) : IIntrantSuggestionService
{
    public async Task<IReadOnlyList<IntrantSuggestionDto>> SearchIntrantsAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var result = await intrants.QueryPagedAsync(
            i => pattern == null || EF.Functions.ILike(i.Nom, pattern),
            query => query.OrderBy(i => i.Nom),
            i => new IntrantSuggestionDto(i.Id, i.Nom, i.Unite),
            page: 1,
            pageSize: 30,
            cancellationToken);

        return result.Items;
    }
}
