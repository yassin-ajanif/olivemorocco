using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Business.Services.Operationnel;

public sealed class InterventionService : IInterventionService
{
    private readonly IRepository<Intervention> _interventions;
    private readonly IRepository<Secteur> _secteurs;
    private readonly IRepository<Intrant> _intrants;
    private readonly IRepository<Charge> _charges;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateInterventionDto>? _createValidator;
    private readonly IValidator<UpdateInterventionDto>? _updateValidator;

    public InterventionService(
        IRepository<Intervention> interventions,
        IRepository<Secteur> secteurs,
        IRepository<Intrant> intrants,
        IRepository<Charge> charges,
        IMapper mapper,
        IEnumerable<IValidator<CreateInterventionDto>> createValidators,
        IEnumerable<IValidator<UpdateInterventionDto>> updateValidators)
    {
        _interventions = interventions;
        _secteurs = secteurs;
        _intrants = intrants;
        _charges = charges;
        _mapper = mapper;
        _createValidator = createValidators.FirstOrDefault();
        _updateValidator = updateValidators.FirstOrDefault();
    }

    public async Task<PagedResult<InterventionListItemDto>> GetInterventionsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var (items, totalCount) = await _interventions.QueryPagedAsync(
            i => pattern == null
                 || EF.Functions.ILike(i.Secteur.Nom, pattern)
                 || (i.Intrant != null && EF.Functions.ILike(i.Intrant.Nom, pattern))
                 || (i.Note != null && EF.Functions.ILike(i.Note, pattern)),
            query => query.OrderByDescending(i => i.Date).ThenByDescending(i => i.Id),
            i => new InterventionListItemDto(
                i.Id,
                i.SecteurId,
                i.Secteur.Nom,
                i.Date,
                i.Intrant != null ? i.Intrant.Nom : null,
                i.Intrant != null ? i.Intrant.Unite : null,
                i.QuantiteIntrant,
                i.QuantiteEau,
                i.Charges.Sum(c => c.MontantTtc),
                i.Note),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<InterventionListItemDto>(items, totalCount);
    }

    public async Task<InterventionDto?> GetInterventionByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _interventions.GetByIdWithNavigationsAsync(
            id,
            [i => i.Secteur, i => i.Intrant!],
            cancellationToken);

        if (entity is null)
            return null;

        var linkedCharges = await _charges.FindWithIncludesAsync(
            c => c.InterventionId == id,
            [c => c.TypeCharge],
            cancellationToken);

        return ToDto(entity, linkedCharges);
    }

    public async Task<InterventionDto> CreateInterventionAsync(
        CreateInterventionDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);
        await EnsureSecteurExistsAsync(dto.SecteurId, cancellationToken);
        await EnsureIntrantExistsAsync(dto.IntrantId, cancellationToken);

        var entity = _mapper.Map<Intervention>(dto);
        NormalizeOptionalFields(entity);

        await _interventions.AddAsync(entity, cancellationToken);
        return (await GetInterventionByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateInterventionAsync(
        int id,
        UpdateInterventionDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, dto, cancellationToken);
        await EnsureSecteurExistsAsync(dto.SecteurId, cancellationToken);
        await EnsureIntrantExistsAsync(dto.IntrantId, cancellationToken);

        var entity = await _interventions.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Intervention {id} introuvable.");

        _mapper.Map(dto, entity);
        NormalizeOptionalFields(entity);

        await _interventions.UpdateAsync(entity, cancellationToken);
    }

    public Task DeleteInterventionAsync(int id, CancellationToken cancellationToken = default)
        => _interventions.DeleteAsync(id, cancellationToken);

    public async Task<IReadOnlyList<SecteurSelectItemDto>> GetSecteursForSelectAsync(
        CancellationToken cancellationToken = default)
    {
        var secteurs = await _secteurs.GetAllAsync(cancellationToken);
        return secteurs
            .OrderBy(s => s.Nom)
            .Select(s => new SecteurSelectItemDto(s.Id, s.Nom))
            .ToList();
    }

    public async Task<IReadOnlyList<IntrantSelectItemDto>> GetIntrantsForSelectAsync(
        CancellationToken cancellationToken = default)
    {
        var intrants = await _intrants.GetAllAsync(cancellationToken);
        return intrants
            .OrderBy(i => i.Nom)
            .Select(i => new IntrantSelectItemDto(i.Id, i.Nom, i.Unite))
            .ToList();
    }

    private static InterventionDto ToDto(Intervention entity, IReadOnlyList<Charge> linkedCharges)
    {
        var charges = linkedCharges
            .OrderByDescending(c => c.Date)
            .ThenByDescending(c => c.Id)
            .Select(c => new InterventionChargeListItemDto(
                c.Id,
                c.TypeCharge.Nom,
                c.Libelle,
                c.Date,
                c.MontantTtc))
            .ToList();

        return new InterventionDto(
            entity.Id,
            entity.SecteurId,
            entity.Secteur.Nom,
            entity.Date,
            entity.IntrantId,
            entity.Intrant?.Nom,
            entity.Intrant?.Unite,
            entity.QuantiteIntrant,
            entity.QuantiteEau,
            entity.Note,
            charges.Sum(c => c.MontantTtc),
            charges);
    }

    private static void NormalizeOptionalFields(Intervention entity)
    {
        if (!entity.IntrantId.HasValue)
        {
            entity.IntrantId = null;
            entity.QuantiteIntrant = null;
        }

        entity.Note = string.IsNullOrWhiteSpace(entity.Note) ? null : entity.Note.Trim();
    }

    private async Task EnsureSecteurExistsAsync(int secteurId, CancellationToken cancellationToken)
    {
        if (!await _secteurs.AnyAsync(s => s.Id == secteurId, cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(CreateInterventionDto.SecteurId),
                    "Le secteur sélectionné est invalide."),
            ]);
        }
    }

    private async Task EnsureIntrantExistsAsync(int? intrantId, CancellationToken cancellationToken)
    {
        if (!intrantId.HasValue)
            return;

        if (!await _intrants.AnyAsync(i => i.Id == intrantId.Value, cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(CreateInterventionDto.IntrantId),
                    "L'intrant sélectionné est invalide."),
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
