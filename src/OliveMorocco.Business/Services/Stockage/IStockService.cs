using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Services.Stockage;

public interface IStockService
{
    Task<PagedResult<StockEtatListItemDto>> GetStockEtatAsync(
        string? search = null,
        bool stockBasOnly = false,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<StockProduitDetailDto?> GetProduitStockDetailAsync(
        int produitId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<MouvementStockListItemDto>> GetMouvementsAsync(
        int produitId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task CreateAjustementAsync(
        int produitId,
        CreateAjustementStockDto dto,
        CancellationToken cancellationToken = default);
}
