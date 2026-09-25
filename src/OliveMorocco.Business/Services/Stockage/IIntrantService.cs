using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Services.Stockage;

public interface IIntrantService
{
    Task<PagedResult<IntrantListItemDto>> GetIntrantsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<IntrantDto?> GetIntrantByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IntrantDto> CreateIntrantAsync(
        CreateIntrantDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateIntrantAsync(
        int id,
        UpdateIntrantDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteIntrantAsync(int id, CancellationToken cancellationToken = default);
}
