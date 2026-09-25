using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Services.Stockage;

public interface ISecteurService
{
    Task<PagedResult<SecteurListItemDto>> GetSecteursAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<SecteurDto?> GetSecteurByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<SecteurDto> CreateSecteurAsync(
        CreateSecteurDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateSecteurAsync(
        int id,
        UpdateSecteurDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteSecteurAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VarieteSelectItemDto>> GetVarietesForSelectAsync(
        CancellationToken cancellationToken = default);
}
