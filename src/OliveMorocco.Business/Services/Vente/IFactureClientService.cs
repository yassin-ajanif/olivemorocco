using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Services.Vente;

public interface IFactureClientService
    : IGenericService<FactureClient, FactureClientDto, CreateFactureClientDto, UpdateFactureClientDto>
{
    Task<PagedResult<FactureClientListItemDto>> GetFacturesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<FactureClientDto?> GetFactureByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<FactureClientDto> CreateFactureAsync(
        CreateFactureClientDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateFactureAsync(
        int id,
        UpdateFactureClientDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteFactureAsync(int id, CancellationToken cancellationToken = default);

    Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default);

    static (decimal Ht, decimal Tva, decimal Ttc) ComputeTotals(
        IEnumerable<CreateFactureClientLigneDto> lignes,
        decimal remiseGlobale) =>
        DocumentTotals.Compute(
            lignes.Select(l => (l.Quantite, l.PrixUnitaireHT, l.Remise, l.TauxTVA)),
            remiseGlobale);
}
