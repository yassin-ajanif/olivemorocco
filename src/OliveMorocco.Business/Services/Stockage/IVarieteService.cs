using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Services.Stockage;

public interface IVarieteService
{
    Task<VarieteDto?> GetVarieteByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<VarieteCreatedDto> CreateVarieteAsync(
        CreateVarieteDto dto,
        CancellationToken cancellationToken = default);

    Task<VarieteDto> UpdateVarieteAsync(
        int id,
        UpdateVarieteDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteVarieteAsync(int id, CancellationToken cancellationToken = default);
}
