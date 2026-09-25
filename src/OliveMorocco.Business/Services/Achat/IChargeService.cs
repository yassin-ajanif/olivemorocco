using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Business.Services.Achat;

public interface IChargeService
{
    Task<PagedResult<ChargeListItemDto>> GetChargesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<ChargeDto?> GetChargeByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ChargeDto> CreateChargeAsync(
        CreateChargeDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateChargeAsync(
        int id,
        UpdateChargeDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteChargeAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TypeChargeSelectItemDto>> GetActiveTypesAsync(
        CancellationToken cancellationToken = default);
}
