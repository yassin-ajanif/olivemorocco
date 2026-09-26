using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Achat;

namespace OliveMorocco.Business.Services.Achat;

public sealed class ChargeService : IChargeService
{
    private readonly IRepository<Charge> _charges;
    private readonly IRepository<TypeCharge> _types;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateChargeDto>? _createValidator;
    private readonly IValidator<UpdateChargeDto>? _updateValidator;

    public ChargeService(
        IRepository<Charge> charges,
        IRepository<TypeCharge> types,
        IMapper mapper,
        IEnumerable<IValidator<CreateChargeDto>> createValidators,
        IEnumerable<IValidator<UpdateChargeDto>> updateValidators)
    {
        _charges = charges;
        _types = types;
        _mapper = mapper;
        _createValidator = createValidators.FirstOrDefault();
        _updateValidator = updateValidators.FirstOrDefault();
    }

    public async Task<PagedResult<ChargeListItemDto>> GetChargesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var (items, totalCount) = await _charges.QueryPagedAsync(
            c => pattern == null
                 || EF.Functions.ILike(c.Libelle, pattern)
                 || EF.Functions.ILike(c.Note, pattern)
                 || EF.Functions.ILike(c.TypeCharge.Nom, pattern),
            query => query.OrderByDescending(c => c.Date).ThenByDescending(c => c.Id),
            c => new ChargeListItemDto(
                c.Id,
                c.TypeCharge.Nom,
                c.Libelle,
                c.Date,
                c.MontantTtc,
                c.InterventionId,
                c.Intervention != null ? c.Intervention.Secteur.Nom : null,
                c.Intervention != null ? c.Intervention.Date : null),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<ChargeListItemDto>(items, totalCount);
    }

    public async Task<ChargeDto?> GetChargeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _charges.GetByIdWithNavigationsAsync(
            id,
            [c => c.TypeCharge],
            cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<ChargeDto> CreateChargeAsync(
        CreateChargeDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);
        await EnsureTypeExistsAsync(dto.TypeChargeId, cancellationToken);

        var entity = _mapper.Map<Charge>(dto);
        entity.Note = dto.Note ?? string.Empty;

        await _charges.AddAsync(entity, cancellationToken);
        return (await GetChargeByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateChargeAsync(
        int id,
        UpdateChargeDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, dto, cancellationToken);
        await EnsureTypeExistsAsync(dto.TypeChargeId, cancellationToken);

        var entity = await _charges.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Charge {id} introuvable.");

        EnsureNotLinkedToIntervention(entity);

        _mapper.Map(dto, entity);
        entity.Note = dto.Note ?? string.Empty;

        await _charges.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteChargeAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _charges.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Charge {id} introuvable.");

        EnsureNotLinkedToIntervention(entity);
        await _charges.DeleteAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<TypeChargeSelectItemDto>> GetActiveTypesAsync(
        CancellationToken cancellationToken = default)
    {
        var types = await _types.FindAsync(t => t.Actif, cancellationToken);
        return types
            .OrderBy(t => t.Nom)
            .Select(t => new TypeChargeSelectItemDto(t.Id, t.Nom))
            .ToList();
    }

    public async Task AddChargesForInterventionAsync(
        int interventionId,
        IReadOnlyList<CreateInterventionChargeDto> charges,
        CancellationToken cancellationToken = default)
    {
        if (charges.Count == 0)
            return;

        foreach (var dto in charges)
        {
            await EnsureTypeExistsAsync(dto.TypeChargeId, cancellationToken);

            var entity = _mapper.Map<Charge>(dto);
            entity.InterventionId = interventionId;
            entity.Note = dto.Note ?? string.Empty;

            await _charges.AddAsync(entity, cancellationToken);
        }
    }

    public async Task ReplaceChargesForInterventionAsync(
        int interventionId,
        IReadOnlyList<CreateInterventionChargeDto> charges,
        CancellationToken cancellationToken = default)
    {
        var existing = await _charges.FindAsync(
            c => c.InterventionId == interventionId,
            cancellationToken);

        foreach (var old in existing)
            await _charges.DeleteAsync(old.Id, cancellationToken);

        await AddChargesForInterventionAsync(interventionId, charges, cancellationToken);
    }

    private static ChargeDto ToDto(Charge entity) =>
        new(
            entity.Id,
            entity.TypeChargeId,
            entity.TypeCharge.Nom,
            entity.InterventionId,
            entity.Libelle,
            entity.Date,
            entity.MontantTtc,
            entity.Note);

    private static void EnsureNotLinkedToIntervention(Charge entity)
    {
        if (entity.InterventionId is not null)
            throw new InvalidOperationException();
    }

    private async Task EnsureTypeExistsAsync(int typeChargeId, CancellationToken cancellationToken)
    {
        if (!await _types.AnyAsync(t => t.Id == typeChargeId && t.Actif, cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(CreateChargeDto.TypeChargeId),
                    "Le type de charge sélectionné est invalide ou inactif."),
            ]);
        }
    }

    private static async Task ValidateAsync<T>(
        IValidator<T>? validator,
        T instance,
        CancellationToken cancellationToken)
    {
        if (validator is null)
            return;

        var result = await validator.ValidateAsync(instance, cancellationToken);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}
