using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Services.Vente;

public interface IAvoirClientService
    : IGenericService<AvoirClient, AvoirClientDto, CreateAvoirClientDto, UpdateAvoirClientDto>
{
    Task<PagedResult<AvoirClientListItemDto>> GetAvoirsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<AvoirClientDto?> GetAvoirByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<AvoirClientDto> CreateAvoirAsync(
        CreateAvoirClientDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateAvoirAsync(
        int id,
        UpdateAvoirClientDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteAvoirAsync(int id, CancellationToken cancellationToken = default);

    Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default);

    static (decimal Ht, decimal Tva, decimal Ttc) ComputeTotals(
        IEnumerable<CreateAvoirClientLigneDto> lignes) =>
        DocumentTotals.Compute(
            lignes.Select(l => (l.Quantite, l.PrixUnitaireHT, l.Remise, l.TauxTVA)));
}
