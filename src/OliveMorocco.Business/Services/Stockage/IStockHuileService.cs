using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Services.Stockage;

public interface IStockHuileService
{
    Task<PagedResult<StockHuileListItemDto>> GetStockHuileAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<StockHuileDetailDto?> GetDetailAsync(
        int varieteId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<MouvementStockListItemDto>> GetMouvementsAsync(
        int varieteId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task CreateAjustementAsync(
        int varieteId,
        CreateAjustementStockDto dto,
        CancellationToken cancellationToken = default);
}
