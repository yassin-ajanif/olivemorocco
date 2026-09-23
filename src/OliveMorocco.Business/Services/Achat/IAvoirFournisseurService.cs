using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Domain.Entities.Achat;

namespace OliveMorocco.Business.Services.Achat;

public interface IAvoirFournisseurService
    : IGenericService<AvoirFournisseur, AvoirFournisseurDto, CreateAvoirFournisseurDto, UpdateAvoirFournisseurDto>
{
    Task<PagedResult<AvoirFournisseurListItemDto>> GetAvoirsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<AvoirFournisseurDto?> GetAvoirByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<AvoirFournisseurDto> CreateAvoirAsync(
        CreateAvoirFournisseurDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateAvoirAsync(
        int id,
        UpdateAvoirFournisseurDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteAvoirAsync(int id, CancellationToken cancellationToken = default);

    Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default);

    static (decimal Ht, decimal Tva, decimal Ttc) ComputeTotals(
        IEnumerable<CreateAvoirFournisseurLigneDto> lignes) =>
        DocumentTotals.Compute(
            lignes.Select(l => (l.Quantite, l.PrixUnitaireHT, l.Remise, l.TauxTVA)));
}
