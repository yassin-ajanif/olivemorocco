using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Services.Vente;

public interface IBonCommandeClientService
    : IGenericService<BonCommandeClient, BonCommandeClientDto, CreateBonCommandeClientDto, UpdateBonCommandeClientDto>
{
    Task<PagedResult<BonCommandeClientListItemDto>> GetBonsCommandeAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<BonCommandeClientDto?> GetBonCommandeByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BonCommandeClientDto> CreateBonCommandeAsync(
        CreateBonCommandeClientDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateBonCommandeAsync(
        int id,
        UpdateBonCommandeClientDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteBonCommandeAsync(int id, CancellationToken cancellationToken = default);

    Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default);

    static (decimal Ht, decimal Tva, decimal Ttc) ComputeTotals(
        IEnumerable<CreateBonCommandeClientLigneDto> lignes) =>
        DocumentTotals.Compute(
            lignes.Select(l => (l.QuantiteCommandee, l.PrixUnitaireHT, l.Remise, l.TauxTVA)));
}
