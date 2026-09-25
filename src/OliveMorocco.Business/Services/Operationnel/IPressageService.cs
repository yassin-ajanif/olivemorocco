using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Services.Operationnel;

public interface IPressageService
{
    Task<PagedResult<PressageListItemDto>> GetPressagesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<PressageDto?> GetPressageByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PressageDto> CreatePressageAsync(
        CreatePressageDto dto,
        CancellationToken cancellationToken = default);

    Task UpdatePressageAsync(
        int id,
        UpdatePressageDto dto,
        CancellationToken cancellationToken = default);

    Task DeletePressageAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FournisseurSelectItemDto>> GetFournisseursForSelectAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VarieteSelectItemDto>> GetVarietesForSelectAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FactureFournisseurSelectItemDto>> GetFacturesForSelectAsync(
        int fournisseurId,
        CancellationToken cancellationToken = default);
}
