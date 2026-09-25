using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Domain.Entities.Achat;

namespace OliveMorocco.Business.Services.Achat;

public interface IBonReceptionService
    : IGenericService<BonReception, BonReceptionDto, CreateBonReceptionDto, UpdateBonReceptionDto>
{
    Task<PagedResult<BonReceptionListItemDto>> GetBonsReceptionAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<BonReceptionDto?> GetBonReceptionByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BonReceptionDto> CreateBonReceptionAsync(
        CreateBonReceptionDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateBonReceptionAsync(
        int id,
        UpdateBonReceptionDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteBonReceptionAsync(int id, CancellationToken cancellationToken = default);

    Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default);

    static (decimal Ht, decimal Tva, decimal Ttc) ComputeTotals(
        IEnumerable<CreateBonReceptionLigneDto> lignes) =>
        DocumentTotals.Compute(
            lignes.Select(l => (l.QuantiteRecue, l.PrixUnitaireHT, 0m, l.TauxTVA)));
}
