using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Services.Stockage;

public interface IVarieteService
{
    Task<VarieteCreatedDto> CreateVarieteAsync(
        CreateVarieteDto dto,
        CancellationToken cancellationToken = default);
}
