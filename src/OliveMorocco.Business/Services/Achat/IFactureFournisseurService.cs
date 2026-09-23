using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Domain.Entities.Achat;

namespace OliveMorocco.Business.Services.Achat;

public interface IFactureFournisseurService
    : IGenericService<FactureFournisseur, FactureFournisseurDto, CreateFactureFournisseurDto, UpdateFactureFournisseurDto>
{
    Task<PagedResult<FactureFournisseurListItemDto>> GetFacturesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<FactureFournisseurDto?> GetFactureByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<FactureFournisseurDto> CreateFactureAsync(
        CreateFactureFournisseurDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateFactureAsync(
        int id,
        UpdateFactureFournisseurDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteFactureAsync(int id, CancellationToken cancellationToken = default);

    Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default);

    static (decimal Ht, decimal Tva, decimal Ttc) ComputeTotals(
        IEnumerable<CreateFactureFournisseurLigneDto> lignes,
        decimal remiseGlobale) =>
        DocumentTotals.Compute(
            lignes.Select(l => (l.Quantite, l.PrixUnitaireHT, l.Remise, l.TauxTVA)),
            remiseGlobale);
}
