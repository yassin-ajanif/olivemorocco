using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Services;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Vente;

public sealed class ClientService
    : GenericService<Tiers, ClientDto, CreateClientDto, UpdateClientDto>,
      IClientService
{
    private readonly ITiersUsageService _tiersUsage;

    public ClientService(
        IRepository<Tiers> tiers,
        ITiersUsageService tiersUsage,
        IMapper mapper,
        IEnumerable<IValidator<CreateClientDto>> createValidators,
        IEnumerable<IValidator<UpdateClientDto>> updateValidators)
        : base(tiers, mapper, createValidators, updateValidators)
    {
        _tiersUsage = tiersUsage;
    }

    public Task<PagedResult<ClientDto>> GetClientsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        return QueryPagedAsync(
            t => (t.Type == TypeTiers.Client || t.Type == TypeTiers.LesDeux)
                 && (pattern == null
                     || EF.Functions.ILike(t.Nom, pattern)
                     || EF.Functions.ILike(t.ICE, pattern)
                     || EF.Functions.ILike(t.Ville, pattern)
                     || EF.Functions.ILike(t.Telephone, pattern)),
            query => query.OrderBy(t => t.Nom),
            t => new ClientDto(
                t.Id,
                t.Type,
                t.Nom,
                t.Adresse,
                t.Ville,
                t.Telephone,
                t.Email,
                t.ICE,
                t.ConditionsPaiement,
                t.Actif),
            page,
            pageSize,
            cancellationToken);
    }

    public async Task<ClientDto?> GetClientByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var dto = await GetByIdAsync(id, cancellationToken);
        return dto is null || !IsClientSide(dto) ? null : dto;
    }

    public async Task<ClientDto> CreateClientAsync(
        CreateClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);

        var entity = Mapper.Map<Tiers>(dto);
        entity.Type = TypeTiers.Client;

        return Mapper.Map<ClientDto>(await Repo.AddAsync(entity, cancellationToken));
    }

    public async Task UpdateClientAsync(
        int id,
        UpdateClientDto dto,
        CancellationToken cancellationToken = default)
    {
        _ = await GetClientByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Client {id} introuvable.");

        await UpdateAsync(id, dto, cancellationToken);
    }

    public async Task DeleteClientAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await GetClientByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Client {id} introuvable.");

        await _tiersUsage.EnsureCanDeleteClientAsync(id, cancellationToken);
        await DeleteAsync(id, cancellationToken);
    }

    public async Task<ClientDto> ToggleActifAsync(int id, CancellationToken cancellationToken = default)
    {
        var dto = await GetClientByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Client {id} introuvable.");

        await UpdateAsync(
            id,
            Mapper.Map<UpdateClientDto>(dto) with { Actif = !dto.Actif },
            cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    private static bool IsClientSide(ClientDto t) =>
        t.Type is TypeTiers.Client or TypeTiers.LesDeux;
}
