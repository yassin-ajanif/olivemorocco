using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.DataAccess;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Business.Services.Stockage;

public sealed class SecteurService : ISecteurService
{
    private readonly AppDbContext _db;
    private readonly IRepository<Secteur> _secteurs;
    private readonly IRepository<Variete> _varietes;
    private readonly IRepository<Intervention> _interventions;
    private readonly IRepository<Recolte> _recoltes;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateSecteurDto>? _createValidator;
    private readonly IValidator<UpdateSecteurDto>? _updateValidator;

    public SecteurService(
        AppDbContext db,
        IRepository<Secteur> secteurs,
        IRepository<Variete> varietes,
        IRepository<Intervention> interventions,
        IRepository<Recolte> recoltes,
        IMapper mapper,
        IEnumerable<IValidator<CreateSecteurDto>> createValidators,
        IEnumerable<IValidator<UpdateSecteurDto>> updateValidators)
    {
        _db = db;
        _secteurs = secteurs;
        _varietes = varietes;
        _interventions = interventions;
        _recoltes = recoltes;
        _mapper = mapper;
        _createValidator = createValidators.FirstOrDefault();
        _updateValidator = updateValidators.FirstOrDefault();
    }

    public async Task<PagedResult<SecteurListItemDto>> GetSecteursAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var query = _db.Secteurs.AsNoTracking();

        if (pattern is not null)
        {
            query = query.Where(s =>
                EF.Functions.ILike(s.Nom, pattern)
                || (s.Code != null && EF.Functions.ILike(s.Code, pattern)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var entities = await query
            .Include(s => s.SecteurVarietes)
            .ThenInclude(sv => sv.Variete)
            .OrderBy(s => s.Nom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = entities
            .Select(s => new SecteurListItemDto(
                s.Id,
                s.Nom,
                s.Code,
                s.SuperficieHectares,
                BuildVarieteSummary(s.SecteurVarietes)))
            .ToList();

        return new PagedResult<SecteurListItemDto>(items, totalCount);
    }

    public async Task<SecteurDto?> GetSecteurByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _secteurs.GetByIdWithNavigationsAsync(
            id,
            [s => s.SecteurVarietes],
            cancellationToken);

        if (entity is null)
            return null;

        var varieteIds = entity.SecteurVarietes.Select(sv => sv.VarieteId).Distinct().ToList();
        var varietes = varieteIds.Count == 0
            ? []
            : await _varietes.FindAsync(v => varieteIds.Contains(v.Id), cancellationToken);
        var varieteMap = varietes.ToDictionary(v => v.Id);

        return ToDto(entity, varieteMap);
    }

    public async Task<SecteurDto> CreateSecteurAsync(
        CreateSecteurDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);
        await EnsureNomUniqueAsync(dto.Nom, excludeId: null, cancellationToken);
        await EnsureCodeUniqueAsync(dto.Code, excludeId: null, cancellationToken);
        await EnsureVarietesExistAsync(dto.Lignes, cancellationToken);
        EnsureAllocationTotalValid(dto.SuperficieHectares, dto.Lignes);

        var entity = _mapper.Map<Secteur>(dto);
        await _secteurs.AddAsync(entity, cancellationToken);

        return (await GetSecteurByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateSecteurAsync(
        int id,
        UpdateSecteurDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, dto, cancellationToken);
        await EnsureNomUniqueAsync(dto.Nom, excludeId: id, cancellationToken);
        await EnsureCodeUniqueAsync(dto.Code, excludeId: id, cancellationToken);
        await EnsureVarietesExistAsync(dto.Lignes, cancellationToken);
        EnsureAllocationTotalValid(dto.SuperficieHectares, dto.Lignes);

        var entity = await _secteurs.GetByIdWithNavigationsAsync(
            id,
            [s => s.SecteurVarietes],
            cancellationToken)
            ?? throw new KeyNotFoundException($"Secteur {id} introuvable.");

        await EnsureRemovedVarietesAllowedAsync(id, entity.SecteurVarietes, dto.Lignes, cancellationToken);

        entity.SecteurVarietes.Clear();
        _mapper.Map(dto, entity);

        foreach (var line in entity.SecteurVarietes)
        {
            line.Id = 0;
            line.SecteurId = entity.Id;
        }

        await _secteurs.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteSecteurAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await _secteurs.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Secteur {id} introuvable.");

        await EnsureCanDeleteAsync(id, cancellationToken);
        await _secteurs.DeleteAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<VarieteSelectItemDto>> GetVarietesForSelectAsync(
        CancellationToken cancellationToken = default)
    {
        var varietes = await _varietes.GetAllAsync(cancellationToken);
        return varietes
            .OrderBy(v => v.Nom)
            .Select(v => new VarieteSelectItemDto(v.Id, v.Nom))
            .ToList();
    }

    private static SecteurDto ToDto(
        Secteur entity,
        IReadOnlyDictionary<int, Variete> varieteMap)
    {
        var lignes = entity.SecteurVarietes
            .OrderBy(sv => varieteMap[sv.VarieteId].Nom)
            .ThenBy(sv => sv.Id)
            .Select(sv => new SecteurVarieteLineDto(
                sv.Id,
                sv.SecteurId,
                sv.VarieteId,
                varieteMap[sv.VarieteId].Nom,
                sv.SuperficieHectares))
            .ToList();

        return new SecteurDto(
            entity.Id,
            entity.Nom,
            entity.Code,
            entity.SuperficieHectares,
            lignes);
    }

    private static string BuildVarieteSummary(IEnumerable<SecteurVariete> lignes)
    {
        var ordered = lignes
            .OrderBy(sv => sv.Variete.Nom)
            .ToList();

        if (ordered.Count == 0)
            return "—";

        return string.Join("; ", ordered.Select(sv =>
            $"{sv.Variete.Nom} {FormatHa(sv.SuperficieHectares)} ha"));
    }

    private static string FormatHa(decimal value) =>
        value.ToString(value % 1 == 0 ? "0" : "0.##");

    private async Task EnsureNomUniqueAsync(
        string nom,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        var normalized = nom.Trim();
        if (await _secteurs.AnyAsync(
                s => s.Nom == normalized && (excludeId == null || s.Id != excludeId),
                cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateSecteurDto.Nom),
                    "Un secteur avec ce nom existe déjà."),
            ]);
        }
    }

    private async Task EnsureCodeUniqueAsync(
        string? code,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;

        var normalized = code.Trim();
        if (await _secteurs.AnyAsync(
                s => s.Code == normalized && (excludeId == null || s.Id != excludeId),
                cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateSecteurDto.Code),
                    "Un secteur avec ce code existe déjà."),
            ]);
        }
    }

    private async Task EnsureVarietesExistAsync(
        IReadOnlyList<CreateSecteurVarieteLineDto> lignes,
        CancellationToken cancellationToken)
    {
        if (lignes.Count == 0)
            return;

        var ids = lignes.Select(l => l.VarieteId).Distinct().ToList();
        var existing = await _varietes.FindAsync(v => ids.Contains(v.Id), cancellationToken);
        if (existing.Count != ids.Count)
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateSecteurDto.Lignes),
                    "Une ou plusieurs variétés sélectionnées sont introuvables."),
            ]);
        }
    }

    private static void EnsureAllocationTotalValid(
        decimal superficieTotale,
        IReadOnlyList<CreateSecteurVarieteLineDto> lignes)
    {
        if (lignes.Count == 0)
            return;

        var total = lignes.Sum(l => l.SuperficieHectares);
        if (total > superficieTotale)
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateSecteurDto.Lignes),
                    $"La superficie allouée aux variétés ({FormatHa(total)} ha) dépasse la superficie totale du secteur ({FormatHa(superficieTotale)} ha)."),
            ]);
        }
    }

    private async Task EnsureRemovedVarietesAllowedAsync(
        int secteurId,
        ICollection<SecteurVariete> existingLines,
        IReadOnlyList<CreateSecteurVarieteLineDto> newLines,
        CancellationToken cancellationToken)
    {
        var newVarieteIds = newLines.Select(l => l.VarieteId).ToHashSet();
        var removedVarieteIds = existingLines
            .Select(l => l.VarieteId)
            .Where(id => !newVarieteIds.Contains(id))
            .ToList();

        if (removedVarieteIds.Count == 0)
            return;

        foreach (var varieteId in removedVarieteIds)
        {
            if (await _recoltes.AnyAsync(
                    r => r.SecteurId == secteurId && r.VarieteId == varieteId,
                    cancellationToken))
            {
                var variete = await _varietes.GetByIdAsync(varieteId, cancellationToken);
                var nom = variete?.Nom ?? $"#{varieteId}";
                throw new ValidationException([
                    new ValidationFailure(
                        nameof(UpdateSecteurDto.Lignes),
                        $"Impossible de retirer la variété « {nom} » : des récoltes y sont liées sur ce secteur."),
                ]);
            }
        }
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await _interventions.AnyAsync(i => i.SecteurId == id, cancellationToken))
            linked.Add("Interventions");
        if (await _recoltes.AnyAsync(r => r.SecteurId == id, cancellationToken))
            linked.Add("Récoltes");

        if (linked.Count == 0)
            return;

        var message = "Impossible de supprimer ce secteur.\n\nDocuments liés :\n"
                      + string.Join('\n', linked.Select(l => $"• {l}"));

        throw new ValidationException([
            new ValidationFailure(string.Empty, message),
        ]);
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
