using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.Services;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Achat;

public sealed class FournisseurService
    : GenericService<Tiers, FournisseurDto, CreateFournisseurDto, UpdateFournisseurDto>,
      IFournisseurService
{
    private readonly ITiersUsageService _tiersUsage;

    public FournisseurService(
        IRepository<Tiers> tiers,
        ITiersUsageService tiersUsage,
        IMapper mapper,
        IEnumerable<IValidator<CreateFournisseurDto>> createValidators,
        IEnumerable<IValidator<UpdateFournisseurDto>> updateValidators)
        : base(tiers, mapper, createValidators, updateValidators)
    {
        _tiersUsage = tiersUsage;
    }

    public Task<PagedResult<FournisseurDto>> GetFournisseursAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        return QueryPagedAsync(
            t => (t.Type == TypeTiers.Fournisseur || t.Type == TypeTiers.LesDeux)
                 && (pattern == null
                     || EF.Functions.ILike(t.Nom, pattern)
                     || EF.Functions.ILike(t.ICE, pattern)
                     || EF.Functions.ILike(t.Ville, pattern)
                     || EF.Functions.ILike(t.Telephone, pattern)),
            query => query.OrderBy(t => t.Nom),
            t => new FournisseurDto(
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

    public async Task<FournisseurDto?> GetFournisseurByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var dto = await GetByIdAsync(id, cancellationToken);
        return dto is null || !IsFournisseurSide(dto) ? null : dto;
    }

    public async Task<FournisseurDto> CreateFournisseurAsync(
        CreateFournisseurDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);

        var entity = Mapper.Map<Tiers>(dto);
        entity.Type = TypeTiers.Fournisseur;

        return Mapper.Map<FournisseurDto>(await Repo.AddAsync(entity, cancellationToken));
    }

    public async Task UpdateFournisseurAsync(
        int id,
        UpdateFournisseurDto dto,
        CancellationToken cancellationToken = default)
    {
        _ = await GetFournisseurByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Fournisseur {id} introuvable.");

        await UpdateAsync(id, dto, cancellationToken);
    }

    public async Task DeleteFournisseurAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await GetFournisseurByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Fournisseur {id} introuvable.");

        await _tiersUsage.EnsureCanDeleteFournisseurAsync(id, cancellationToken);
        await DeleteAsync(id, cancellationToken);
    }

    public async Task<FournisseurDto> ToggleActifAsync(int id, CancellationToken cancellationToken = default)
    {
        var dto = await GetFournisseurByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Fournisseur {id} introuvable.");

        await UpdateAsync(
            id,
            Mapper.Map<UpdateFournisseurDto>(dto) with { Actif = !dto.Actif },
            cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    private static bool IsFournisseurSide(FournisseurDto t) =>
        t.Type is TypeTiers.Fournisseur or TypeTiers.LesDeux;
}
