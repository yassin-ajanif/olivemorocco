using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Domain.Entities.Achat;

namespace OliveMorocco.Business.Services.Achat;

public interface IBonCommandeFournisseurService
    : IGenericService<BonCommandeFournisseur, BonCommandeFournisseurDto, CreateBonCommandeFournisseurDto, UpdateBonCommandeFournisseurDto>
{
    Task<PagedResult<BonCommandeFournisseurListItemDto>> GetBonsCommandeAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<BonCommandeFournisseurDto?> GetBonCommandeByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BonCommandeFournisseurDto> CreateBonCommandeAsync(
        CreateBonCommandeFournisseurDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateBonCommandeAsync(
        int id,
        UpdateBonCommandeFournisseurDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteBonCommandeAsync(int id, CancellationToken cancellationToken = default);

    Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default);

    static (decimal Ht, decimal Tva, decimal Ttc) ComputeTotals(
        IEnumerable<CreateBonCommandeFournisseurLigneDto> lignes) =>
        DocumentTotals.Compute(
            lignes.Select(l => (l.QuantiteCommandee, l.PrixUnitaireHT, l.Remise, l.TauxTVA)));
}
