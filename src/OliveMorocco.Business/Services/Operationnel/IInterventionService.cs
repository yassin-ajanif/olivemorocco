using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Services.Operationnel;

public interface IInterventionService
{
    Task<PagedResult<InterventionListItemDto>> GetInterventionsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<InterventionDto?> GetInterventionByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<InterventionDto> CreateInterventionAsync(
        CreateInterventionDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateInterventionAsync(
        int id,
        UpdateInterventionDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteInterventionAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SecteurSelectItemDto>> GetSecteursForSelectAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IntrantSelectItemDto>> GetIntrantsForSelectAsync(
        CancellationToken cancellationToken = default);
}
