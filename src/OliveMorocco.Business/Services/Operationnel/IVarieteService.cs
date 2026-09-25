using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Services.Operationnel;

public interface IVarieteService
{
    Task<VarieteCreatedDto> CreateVarieteAsync(
        CreateVarieteDto dto,
        CancellationToken cancellationToken = default);
}
