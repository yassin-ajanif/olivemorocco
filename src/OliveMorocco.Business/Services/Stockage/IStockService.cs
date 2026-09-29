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

    Task ApplyBonLivraisonSortieAsync(
        int bonLivraisonId,
        IEnumerable<(int ProduitId, decimal QuantiteLivree)> lignes,
        CancellationToken cancellationToken = default);

    Task ApplyBonLivraisonAjustementAsync(
        int bonLivraisonId,
        IEnumerable<(int ProduitId, decimal Delta)> deltas,
        CancellationToken cancellationToken = default);

    Task ReverseBonLivraisonAsync(
        int bonLivraisonId,
        IEnumerable<(int ProduitId, decimal QuantiteLivree)> lignes,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies a product stock movement to the ledger: rejects a negative resulting stock
    /// and persists the <c>MouvementStock</c>. Product stock is the sum of those movements.
    /// Used by the operational flows (remplissage) so the ledger has a single writer.
    /// </summary>
    /// <param name="errorPropertyName">Validation property the "insufficient stock" failure is attached to.</param>
    Task ApplyProduitMouvementAsync(
        int produitId,
        decimal variation,
        string origineType,
        int? origineId,
        string? note,
        string errorPropertyName,
        CancellationToken cancellationToken = default);
}
