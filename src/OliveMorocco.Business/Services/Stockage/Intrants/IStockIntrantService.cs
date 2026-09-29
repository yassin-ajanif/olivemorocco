using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Services.Stockage.Intrants;

public interface IStockIntrantService
{
    Task<PagedResult<IntrantStockListItemDto>> GetStockIntrantAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<IntrantStockDetailDto?> GetDetailAsync(
        int intrantId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<MouvementIntrantListItemDto>> GetMouvementsAsync(
        int intrantId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task CreateAjustementAsync(
        int intrantId,
        CreateAjustementIntrantDto dto,
        CancellationToken cancellationToken = default);

    Task ApplyInterventionSortieAsync(
        int interventionId,
        IEnumerable<(int IntrantId, decimal Quantite)> lignes,
        CancellationToken cancellationToken = default);

    Task ApplyInterventionAjustementAsync(
        int interventionId,
        IEnumerable<(int IntrantId, decimal Delta)> deltas,
        CancellationToken cancellationToken = default);

    Task ReverseInterventionAsync(
        int interventionId,
        IEnumerable<(int IntrantId, decimal Quantite)> lignes,
        CancellationToken cancellationToken = default);
}
