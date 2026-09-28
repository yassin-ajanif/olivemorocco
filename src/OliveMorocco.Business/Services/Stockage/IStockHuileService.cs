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

    /// <summary>
    /// Applies a bulk-oil movement to the variété ledger: rejects a negative resulting stock,
    /// keeps <c>Variete.StockHuile</c> in sync and persists the <c>MouvementStockVariete</c>.
    /// Used by the operational flows (pressage, remplissage) so the ledger has a single writer.
    /// </summary>
    /// <param name="errorPropertyName">Validation property the "insufficient stock" failure is attached to.</param>
    Task ApplyVarieteMouvementAsync(
        int varieteId,
        decimal variation,
        string origineType,
        int? origineId,
        string? note,
        string errorPropertyName,
        CancellationToken cancellationToken = default);
}
