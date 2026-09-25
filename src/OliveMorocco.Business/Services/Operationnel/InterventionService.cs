using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Business.Services.Achat;
using OliveMorocco.DataAccess;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Business.Services.Operationnel;

public sealed class InterventionService : IInterventionService
{
    private readonly AppDbContext _db;
    private readonly IRepository<Intervention> _interventions;
    private readonly IRepository<Secteur> _secteurs;
    private readonly IRepository<Intrant> _intrants;
    private readonly IRepository<Charge> _charges;
    private readonly IChargeService _chargeService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateInterventionDto>? _createValidator;
    private readonly IValidator<UpdateInterventionDto>? _updateValidator;

    public InterventionService(
        AppDbContext db,
        IRepository<Intervention> interventions,
        IRepository<Secteur> secteurs,
        IRepository<Intrant> intrants,
        IRepository<Charge> charges,
        IChargeService chargeService,
        IMapper mapper,
        IEnumerable<IValidator<CreateInterventionDto>> createValidators,
        IEnumerable<IValidator<UpdateInterventionDto>> updateValidators)
    {
        _db = db;
        _interventions = interventions;
        _secteurs = secteurs;
        _intrants = intrants;
        _charges = charges;
        _chargeService = chargeService;
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
                 || i.Lignes.Any(l => EF.Functions.ILike(l.Intrant.Nom, pattern))
                 || (i.Note != null && EF.Functions.ILike(i.Note, pattern)),
            query => query.OrderByDescending(i => i.Date).ThenByDescending(i => i.Id),
            i => new InterventionListItemDto(
                i.Id,
                i.SecteurId,
                i.Secteur.Nom,
                i.Date,
                !i.Lignes.Any()
                    ? "—"
                    : string.Join("; ", i.Lignes
                        .OrderBy(l => l.Intrant.Nom)
                        .Select(l => l.Intrant.Nom + " " + l.Quantite + " " + l.Intrant.Unite)),
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
            [i => i.Secteur, i => i.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var intrantIds = entity.Lignes.Select(l => l.IntrantId).Distinct().ToList();
        var intrants = intrantIds.Count == 0
            ? []
            : await _intrants.FindAsync(i => intrantIds.Contains(i.Id), cancellationToken);
        var intrantMap = intrants.ToDictionary(i => i.Id);

        var linkedCharges = await _charges.FindWithIncludesAsync(
            c => c.InterventionId == id,
            [c => c.TypeCharge],
            cancellationToken);

        return ToDto(entity, intrantMap, linkedCharges);
    }

    public async Task<InterventionDto> CreateInterventionAsync(
        CreateInterventionDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);
        await EnsureSecteurExistsAsync(dto.SecteurId, cancellationToken);
        await EnsureIntrantsExistAsync(dto.Lignes, cancellationToken);

        await using var transaction =
            await _db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var interventionId = await AddInterventionCoreAsync(dto, cancellationToken);

            await _chargeService.AddChargesForInterventionAsync(
                interventionId,
                dto.Charges,
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return (await GetInterventionByIdAsync(interventionId, cancellationToken))!;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateInterventionAsync(
        int id,
        UpdateInterventionDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, dto, cancellationToken);
        await EnsureSecteurExistsAsync(dto.SecteurId, cancellationToken);
        await EnsureIntrantsExistAsync(dto.Lignes, cancellationToken);

        _ = await _interventions.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Intervention {id} introuvable.");

        await using var transaction =
            await _db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await UpdateInterventionCoreAsync(id, dto, cancellationToken);

            await _chargeService.ReplaceChargesForInterventionAsync(
                id,
                dto.Charges,
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
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

    private async Task<int> AddInterventionCoreAsync(
        CreateInterventionDto dto,
        CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Intervention>(dto);
        NormalizeOptionalFields(entity);

        await _interventions.AddAsync(entity, cancellationToken);
        return entity.Id;
    }

    private async Task UpdateInterventionCoreAsync(
        int id,
        UpdateInterventionDto dto,
        CancellationToken cancellationToken)
    {
        var entity = await _interventions.GetByIdWithNavigationsAsync(id, [i => i.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Intervention {id} introuvable.");

        entity.Lignes.Clear();
        _mapper.Map(dto, entity);
        NormalizeOptionalFields(entity);

        foreach (var line in entity.Lignes)
        {
            line.Id = 0;
            line.InterventionId = entity.Id;
        }

        await _interventions.UpdateAsync(entity, cancellationToken);
    }

    private static InterventionDto ToDto(
        Intervention entity,
        IReadOnlyDictionary<int, Intrant> intrantMap,
        IReadOnlyList<Charge> linkedCharges)
    {
        var lignes = entity.Lignes
            .OrderBy(l => intrantMap[l.IntrantId].Nom)
            .ThenBy(l => l.Id)
            .Select(l => new InterventionLigneDto(
                l.Id,
                l.InterventionId,
                l.IntrantId,
                intrantMap[l.IntrantId].Nom,
                intrantMap[l.IntrantId].Unite,
                l.Quantite))
            .ToList();

        var charges = linkedCharges
            .OrderByDescending(c => c.Date)
            .ThenByDescending(c => c.Id)
            .Select(c => new InterventionChargeListItemDto(
                c.Id,
                c.TypeChargeId,
                c.TypeCharge.Nom,
                c.Libelle,
                c.Date,
                c.MontantTtc,
                string.IsNullOrEmpty(c.Note) ? null : c.Note))
            .ToList();

        return new InterventionDto(
            entity.Id,
            entity.SecteurId,
            entity.Secteur.Nom,
            entity.Date,
            entity.QuantiteEau,
            entity.Note,
            charges.Sum(c => c.MontantTtc),
            lignes,
            charges);
    }

    private static void NormalizeOptionalFields(Intervention entity)
    {
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

    private async Task EnsureIntrantsExistAsync(
        IReadOnlyList<CreateInterventionLigneDto> lignes,
        CancellationToken cancellationToken)
    {
        foreach (var ligne in lignes)
        {
            if (!await _intrants.AnyAsync(i => i.Id == ligne.IntrantId, cancellationToken))
            {
                throw new ValidationException([
                    new ValidationFailure(nameof(CreateInterventionLigneDto.IntrantId),
                        "Un intrant sélectionné est invalide."),
                ]);
            }
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
