using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Vente;

namespace OliveMorocco.Business.Services.Vente;

public interface IProduitService
{
    Task<PagedResult<ProduitListItemDto>> GetProduitsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<ProduitDto?> GetProduitByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VarieteSelectItemDto>> GetVarietesForSelectAsync(
        CancellationToken cancellationToken = default);

    Task<ProduitDto> CreateProduitAsync(
        CreateProduitDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateProduitAsync(
        int id,
        UpdateProduitDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteProduitAsync(int id, CancellationToken cancellationToken = default);

    Task<ProduitDto> ToggleActifAsync(int id, CancellationToken cancellationToken = default);
}
