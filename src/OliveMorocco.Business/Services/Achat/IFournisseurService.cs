using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.Services;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Business.Services.Achat;

public interface IFournisseurService : IGenericService<Tiers, FournisseurDto, CreateFournisseurDto, UpdateFournisseurDto>
{
    Task<PagedResult<FournisseurDto>> GetFournisseursAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<FournisseurDto?> GetFournisseurByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FournisseurDto> CreateFournisseurAsync(CreateFournisseurDto dto, CancellationToken cancellationToken = default);
    Task UpdateFournisseurAsync(int id, UpdateFournisseurDto dto, CancellationToken cancellationToken = default);
    Task DeleteFournisseurAsync(int id, CancellationToken cancellationToken = default);
    Task<FournisseurDto> ToggleActifAsync(int id, CancellationToken cancellationToken = default);
}
