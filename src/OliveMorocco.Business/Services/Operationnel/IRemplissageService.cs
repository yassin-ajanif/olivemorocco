using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Services.Operationnel;

public interface IRemplissageService
{
    Task<PagedResult<RemplissageListItemDto>> GetRemplissagesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<RemplissageDto?> GetRemplissageByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<RemplissageDto> CreateRemplissageAsync(
        CreateRemplissageDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateRemplissageAsync(
        int id,
        UpdateRemplissageDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteRemplissageAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RemplissageVarieteSelectItemDto>> GetVarietesForSelectAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Active products with a <c>ContenanceLitres</c>, fillable from bulk oil.</summary>
    Task<IReadOnlyList<RemplissageProduitSelectItemDto>> GetProduitsForSelectAsync(
        CancellationToken cancellationToken = default);
}
