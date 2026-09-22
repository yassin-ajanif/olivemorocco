using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Services;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Business.Services.Vente;

public interface IClientService : IGenericService<Tiers, ClientDto, CreateClientDto, UpdateClientDto>
{
    Task<PagedResult<ClientDto>> GetClientsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<ClientDto?> GetClientByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ClientDto> CreateClientAsync(CreateClientDto dto, CancellationToken cancellationToken = default);
    Task UpdateClientAsync(int id, UpdateClientDto dto, CancellationToken cancellationToken = default);
    Task DeleteClientAsync(int id, CancellationToken cancellationToken = default);
    Task<ClientDto> ToggleActifAsync(int id, CancellationToken cancellationToken = default);
}
