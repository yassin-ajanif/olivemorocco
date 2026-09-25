using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Services.Vente;

public interface IDevisClientService
    : IGenericService<DevisClient, DevisClientDto, CreateDevisClientDto, UpdateDevisClientDto>
{
    Task<PagedResult<DevisClientListItemDto>> GetDevisAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<DevisClientDto?> GetDevisByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<DevisClientDto> CreateDevisAsync(
        CreateDevisClientDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateDevisAsync(
        int id,
        UpdateDevisClientDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteDevisAsync(int id, CancellationToken cancellationToken = default);

    Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default);

    static (decimal Ht, decimal Tva, decimal Ttc) ComputeTotals(
        IEnumerable<CreateDevisClientLigneDto> lignes,
        decimal remiseGlobale) =>
        DocumentTotals.Compute(
            lignes.Select(l => (l.Quantite, l.PrixUnitaireHT, l.Remise, l.TauxTVA)),
            remiseGlobale);
}
