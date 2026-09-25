using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Services.Vente;

public interface IBonLivraisonClientService
    : IGenericService<BonLivraisonClient, BonLivraisonClientDto, CreateBonLivraisonClientDto, UpdateBonLivraisonClientDto>
{
    Task<PagedResult<BonLivraisonClientListItemDto>> GetBonsLivraisonAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<BonLivraisonClientDto?> GetBonLivraisonByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BonLivraisonClientDto> CreateBonLivraisonAsync(
        CreateBonLivraisonClientDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateBonLivraisonAsync(
        int id,
        UpdateBonLivraisonClientDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteBonLivraisonAsync(int id, CancellationToken cancellationToken = default);

    Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default);

    static (decimal Ht, decimal Tva, decimal Ttc) ComputeTotals(
        IEnumerable<CreateBonLivraisonClientLigneDto> lignes) =>
        DocumentTotals.Compute(
            lignes.Select(l => (l.QuantiteLivree, l.PrixUnitaireHT, l.Remise, l.TauxTVA)));
}
