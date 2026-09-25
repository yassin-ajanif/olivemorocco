using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Business.DTOs.Stockage;
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
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePressageDto>? _createValidator;
    private readonly IValidator<UpdatePressageDto>? _updateValidator;

    public PressageService(
        IRepository<Pressage> pressages,
        IRepository<Tiers> tiers,
        IRepository<Variete> varietes,
        IRepository<FactureFournisseur> factures,
        IMapper mapper,
        IEnumerable<IValidator<CreatePressageDto>> createValidators,
        IEnumerable<IValidator<UpdatePressageDto>> updateValidators)
    {
        _pressages = pressages;
        _tiers = tiers;
        _varietes = varietes;
        _factures = factures;
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
                 || EF.Functions.ILike(p.Variete.Nom, pattern)
                 || (p.FactureFournisseur != null && EF.Functions.ILike(p.FactureFournisseur.Numero, pattern)),
            query => query.OrderByDescending(p => p.Date).ThenByDescending(p => p.Id),
            p => new PressageListItemDto(
                p.Id,
                p.Date,
                p.Fournisseur.Nom,
                p.Variete.Nom,
                p.QuantiteOlives,
                p.Rendement,
                p.QuantiteHuile,
                p.FactureFournisseur != null ? p.FactureFournisseur.Numero : null),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<PressageListItemDto>(items, totalCount);
    }

    public async Task<PressageDto?> GetPressageByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _pressages.GetByIdWithNavigationsAsync(
            id,
            [p => p.Fournisseur, p => p.Variete, p => p.FactureFournisseur!],
            cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<PressageDto> CreatePressageAsync(
        CreatePressageDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);
        await EnsureReferencesValidAsync(dto.FournisseurId, dto.VarieteId, dto.FactureFournisseurId, cancellationToken);

        var entity = _mapper.Map<Pressage>(dto);
        entity.QuantiteHuile = ResolveQuantiteHuile(dto.QuantiteOlives, dto.Rendement, dto.QuantiteHuile);

        await _pressages.AddAsync(entity, cancellationToken);

        return (await GetPressageByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdatePressageAsync(
        int id,
        UpdatePressageDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, dto, cancellationToken);
        await EnsureReferencesValidAsync(dto.FournisseurId, dto.VarieteId, dto.FactureFournisseurId, cancellationToken);

        var entity = await _pressages.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Pressage {id} introuvable.");

        _mapper.Map(dto, entity);
        entity.QuantiteHuile = ResolveQuantiteHuile(dto.QuantiteOlives, dto.Rendement, dto.QuantiteHuile);

        await _pressages.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeletePressageAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await _pressages.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Pressage {id} introuvable.");

        await _pressages.DeleteAsync(id, cancellationToken);
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

    public async Task<IReadOnlyList<FactureFournisseurSelectItemDto>> GetFacturesForSelectAsync(
        int fournisseurId,
        CancellationToken cancellationToken = default)
    {
        if (fournisseurId <= 0)
            return [];

        var factures = await _factures.FindAsync(
            f => f.FournisseurId == fournisseurId,
            cancellationToken);

        return factures
            .OrderByDescending(f => f.Date)
            .ThenByDescending(f => f.Id)
            .Select(f => new FactureFournisseurSelectItemDto(f.Id, f.Numero, f.Date))
            .ToList();
    }

    private static PressageDto ToDto(Pressage entity) =>
        new(
            entity.Id,
            entity.FournisseurId,
            entity.Fournisseur.Nom,
            entity.VarieteId,
            entity.Variete.Nom,
            entity.Date,
            entity.QuantiteOlives,
            entity.Rendement,
            entity.QuantiteHuile,
            entity.FactureFournisseurId,
            entity.FactureFournisseur?.Numero);

    private static decimal? ResolveQuantiteHuile(
        decimal quantiteOlives,
        decimal rendement,
        decimal? quantiteHuile)
    {
        if (quantiteHuile.HasValue)
            return quantiteHuile;

        return Math.Round(quantiteOlives * rendement / 100m, 4);
    }

    private async Task EnsureReferencesValidAsync(
        int fournisseurId,
        int varieteId,
        int? factureFournisseurId,
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

        if (factureFournisseurId is null)
            return;

        var facture = await _factures.GetByIdAsync(factureFournisseurId.Value, cancellationToken);
        if (facture is null || facture.FournisseurId != fournisseurId)
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreatePressageDto.FactureFournisseurId),
                    "La facture sélectionnée n'appartient pas à cette huilerie."),
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
