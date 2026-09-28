using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.Business.Services.Stockage;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Operationnel;

public sealed class PressageService : IPressageService
{
    private readonly IRepository<Pressage> _pressages;
    private readonly IRepository<Tiers> _tiers;
    private readonly IRepository<Variete> _varietes;
    private readonly IRepository<FactureFournisseur> _factures;
    private readonly IRepository<MouvementStockVariete> _mouvementsHuile;
    private readonly IRepository<Charge> _charges;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePressageDto>? _createValidator;
    private readonly IValidator<UpdatePressageDto>? _updateValidator;

    public PressageService(
        IRepository<Pressage> pressages,
        IRepository<Tiers> tiers,
        IRepository<Variete> varietes,
        IRepository<FactureFournisseur> factures,
        IRepository<MouvementStockVariete> mouvementsHuile,
        IRepository<Charge> charges,
        IMapper mapper,
        IEnumerable<IValidator<CreatePressageDto>> createValidators,
        IEnumerable<IValidator<UpdatePressageDto>> updateValidators)
    {
        _pressages = pressages;
        _tiers = tiers;
        _varietes = varietes;
        _factures = factures;
        _mouvementsHuile = mouvementsHuile;
        _charges = charges;
        _mapper = mapper;
        _createValidator = createValidators.FirstOrDefault();
        _updateValidator = updateValidators.FirstOrDefault();
    }

    public async Task<PagedResult<PressageListItemDto>> GetPressagesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var (items, totalCount) = await _pressages.QueryPagedAsync(
            p => pattern == null
                 || EF.Functions.ILike(p.Fournisseur.Nom, pattern)
                 || EF.Functions.ILike(p.Variete.Nom, pattern),
            query => query.OrderByDescending(p => p.Date).ThenByDescending(p => p.Id),
            p => new PressageListItemDto(
                p.Id,
                p.Numero,
                p.Date,
                p.Fournisseur.Nom,
                p.Variete.Nom,
                p.QuantiteOlives,
                p.Rendement,
                p.QuantiteHuile),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<PressageListItemDto>(items, totalCount);
    }

    public async Task<PressageDto?> GetPressageByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _pressages.GetByIdWithNavigationsAsync(
            id,
            [p => p.Fournisseur, p => p.Variete],
            cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<PressageDto> CreatePressageAsync(
        CreatePressageDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);
        await EnsureReferencesValidAsync(dto.FournisseurId, dto.VarieteId, cancellationToken);

        var entity = _mapper.Map<Pressage>(dto);
        entity.Numero = await GenerateNumeroAsync(cancellationToken);
        entity.QuantiteHuile = ResolveQuantiteHuile(dto.QuantiteOlives, dto.Rendement, dto.QuantiteHuile);

        var charge = new Charge
        {
            TypeChargeId = dto.TypeChargeId,
            Libelle = dto.Libelle,
            Date = dto.ChargeDate,
            MontantTtc = dto.MontantTtc,
            Note = dto.Note ?? string.Empty,
        };

        await _pressages.ExecuteInTransactionAsync(async ct =>
        {
            await _charges.AddAsync(charge, ct);
            entity.ChargeId = charge.Id;
            await _pressages.AddAsync(entity, ct);
            await ApplyHuileAsync(
                entity.VarieteId,
                entity.QuantiteHuile ?? 0m,
                entity.Id,
                string.Empty,
                ct);
        }, cancellationToken);

        return (await GetPressageByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdatePressageAsync(
        int id,
        UpdatePressageDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, dto, cancellationToken);
        await EnsureReferencesValidAsync(dto.FournisseurId, dto.VarieteId, cancellationToken);

        var entity = await _pressages.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Pressage {id} introuvable.");

        var ancienneVarieteId = entity.VarieteId;
        var ancienneHuile = entity.QuantiteHuile ?? 0m;

        _mapper.Map(dto, entity);
        entity.QuantiteHuile = ResolveQuantiteHuile(dto.QuantiteOlives, dto.Rendement, dto.QuantiteHuile);
        var nouvelleHuile = entity.QuantiteHuile ?? 0m;

        await _pressages.ExecuteInTransactionAsync(async ct =>
        {
            const string note = "Modification du pressage";

            if (ancienneVarieteId == entity.VarieteId)
            {
                await ApplyHuileAsync(entity.VarieteId, nouvelleHuile - ancienneHuile, id, note, ct);
            }
            else
            {
                await ApplyHuileAsync(ancienneVarieteId, -ancienneHuile, id, note, ct);
                await ApplyHuileAsync(entity.VarieteId, nouvelleHuile, id, note, ct);
            }

            await _pressages.UpdateAsync(entity, ct);
        }, cancellationToken);
    }

    public async Task DeletePressageAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _pressages.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Pressage {id} introuvable.");

        await _pressages.ExecuteInTransactionAsync(async ct =>
        {
            await ApplyHuileAsync(
                entity.VarieteId,
                -(entity.QuantiteHuile ?? 0m),
                id,
                "Suppression du pressage",
                ct);
            await _pressages.DeleteAsync(id, ct);
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<FournisseurSelectItemDto>> GetFournisseursForSelectAsync(
        CancellationToken cancellationToken = default)
    {
        var fournisseurs = await _tiers.FindAsync(
            t => (t.Type == TypeTiers.Fournisseur || t.Type == TypeTiers.LesDeux) && t.Actif,
            cancellationToken);

        return fournisseurs
            .OrderBy(t => t.Nom)
            .Select(t => new FournisseurSelectItemDto(t.Id, t.Nom))
            .ToList();
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

    private static PressageDto ToDto(Pressage entity) =>
        new(
            entity.Id,
            entity.Numero,
            entity.FournisseurId,
            entity.Fournisseur.Nom,
            entity.VarieteId,
            entity.Variete.Nom,
            entity.Date,
            entity.QuantiteOlives,
            entity.Rendement,
            entity.QuantiteHuile);

    private async Task<string> GenerateNumeroAsync(CancellationToken cancellationToken)
    {
        var prefix = $"PRS-{DateTime.Today.Year}-";
        var existing = await _pressages.FindAsync(p => p.Numero.StartsWith(prefix), cancellationToken);
        var next = existing
            .Select(p => int.TryParse(p.Numero[prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"{prefix}{next:D4}";
    }

    private static decimal? ResolveQuantiteHuile(
        decimal quantiteOlives,
        decimal rendement,
        decimal? quantiteHuile)
    {
        if (quantiteHuile.HasValue)
            return quantiteHuile;

        return Math.Round(quantiteOlives * rendement / 100m, 4);
    }

    private async Task ApplyHuileAsync(
        int varieteId,
        decimal variation,
        int pressageId,
        string note,
        CancellationToken cancellationToken)
    {
        if (variation == 0)
            return;

        var variete = await _varietes.GetByIdAsync(varieteId, cancellationToken)
            ?? throw new KeyNotFoundException($"Variété {varieteId} introuvable.");

        var mouvement = StockHuileMouvements.Apply(
            variete,
            variation,
            StockHuileMouvements.OriginePressage,
            pressageId,
            note,
            nameof(CreatePressageDto.QuantiteHuile),
            $"Stock d'huile insuffisant pour « {variete.Nom} » : cette huile a déjà été utilisée ou ajustée.");

        await _mouvementsHuile.AddAsync(mouvement, cancellationToken);
    }

    private async Task EnsureReferencesValidAsync(
        int fournisseurId,
        int varieteId,
        CancellationToken cancellationToken)
    {
        if (!await _tiers.AnyAsync(
                t => t.Id == fournisseurId
                     && (t.Type == TypeTiers.Fournisseur || t.Type == TypeTiers.LesDeux),
                cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreatePressageDto.FournisseurId),
                    "Huilerie (fournisseur) introuvable."),
            ]);
        }

        if (!await _varietes.AnyAsync(v => v.Id == varieteId, cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreatePressageDto.VarieteId),
                    "Variété introuvable."),
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
