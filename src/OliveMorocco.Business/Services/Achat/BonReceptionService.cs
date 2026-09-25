using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Achat;

public sealed class BonReceptionService
    : GenericService<BonReception, BonReceptionDto, CreateBonReceptionDto, UpdateBonReceptionDto>,
      IBonReceptionService
{
    private readonly IRepository<BonReceptionLigne> _lignes;
    private readonly IRepository<Tiers> _tiers;
    private readonly IRepository<FactureFournisseurLigne> _factureLignes;
    private readonly IRepository<FactureFournisseur> _factures;

    public BonReceptionService(
        IRepository<BonReception> bons,
        IRepository<BonReceptionLigne> lignes,
        IRepository<Tiers> tiers,
        IRepository<FactureFournisseurLigne> factureLignes,
        IRepository<FactureFournisseur> factures,
        IMapper mapper,
        IEnumerable<IValidator<CreateBonReceptionDto>> createValidators,
        IEnumerable<IValidator<UpdateBonReceptionDto>> updateValidators)
        : base(bons, mapper, createValidators, updateValidators)
    {
        _lignes = lignes;
        _tiers = tiers;
        _factureLignes = factureLignes;
        _factures = factures;
    }

    public async Task<PagedResult<BonReceptionListItemDto>> GetBonsReceptionAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var predicate = (System.Linq.Expressions.Expression<Func<BonReception, bool>>?)(b =>
            pattern == null
            || EF.Functions.ILike(b.Numero, pattern)
            || EF.Functions.ILike(b.Fournisseur.Nom, pattern));

        var (items, totalCount) = await Repo.QueryPagedAsync(
            predicate,
            query => query.OrderByDescending(b => b.Date).ThenByDescending(b => b.Id),
            b => b,
            page,
            pageSize,
            cancellationToken);

        var ids = items.Select(b => b.Id).ToList();
        if (ids.Count == 0)
            return new PagedResult<BonReceptionListItemDto>([], totalCount);

        var headers = await Repo.FindWithIncludesAsync(
            b => ids.Contains(b.Id),
            [b => b.Fournisseur, b => b.Lignes],
            cancellationToken);

        var byId = headers.ToDictionary(b => b.Id);
        var factureIds = headers
            .Where(b => b.FactureFournisseurId is not null)
            .Select(b => b.FactureFournisseurId!.Value)
            .Distinct()
            .ToList();
        var factureNumeros = factureIds.Count == 0
            ? new Dictionary<int, string>()
            : (await _factures.FindAsync(f => factureIds.Contains(f.Id), cancellationToken))
                .ToDictionary(f => f.Id, f => f.Numero);

        var listItems = items
            .Where(b => byId.ContainsKey(b.Id))
            .Select(b =>
            {
                var full = byId[b.Id];
                var (_, _, ttc) = IBonReceptionService.ComputeTotals(
                    full.Lignes.Select(l => new CreateBonReceptionLigneDto(
                        l.IntrantId,
                        l.Designation,
                        l.QuantiteRecue,
                        l.PrixUnitaireHT,
                        l.TauxTVA)));

                string? factureNumero = null;
                if (full.FactureFournisseurId is int factureId && factureNumeros.TryGetValue(factureId, out var numero))
                    factureNumero = numero;

                return new BonReceptionListItemDto(
                    full.Id,
                    full.Numero,
                    full.FournisseurId,
                    full.Fournisseur.Nom,
                    full.Date,
                    ttc,
                    full.Note,
                    full.FactureFournisseurId,
                    factureNumero);
            })
            .ToList();

        return new PagedResult<BonReceptionListItemDto>(listItems, totalCount);
    }

    public async Task<BonReceptionDto?> GetBonReceptionByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdWithNavigationsAsync(
            id,
            [b => b.Fournisseur, b => b.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var ligneDtos = await _lignes.FindWithIncludesAsync(
            l => l.BRId == id,
            [l => l.Intrant!],
            cancellationToken);

        var (_, _, ttc) = IBonReceptionService.ComputeTotals(
            ligneDtos.Select(l => new CreateBonReceptionLigneDto(
                l.IntrantId,
                l.Designation,
                l.QuantiteRecue,
                l.PrixUnitaireHT,
                l.TauxTVA)));

        string? factureNumero = null;
        if (entity.FactureFournisseurId is int factureId)
        {
            var facture = await _factures.GetByIdAsync(factureId, cancellationToken);
            factureNumero = facture?.Numero;
        }

        return new BonReceptionDto(
            entity.Id,
            entity.Numero,
            entity.FournisseurId,
            entity.BonCommandeId,
            entity.FactureFournisseurId,
            factureNumero,
            entity.Date,
            ttc,
            entity.Note,
            ligneDtos.Select(l => new BonReceptionLigneDto(
                l.Id,
                l.BRId,
                l.IntrantId,
                string.Empty,
                l.Designation,
                l.QuantiteRecue,
                l.PrixUnitaireHT,
                l.TauxTVA)).ToList());
    }

    public async Task<BonReceptionDto> CreateBonReceptionAsync(
        CreateBonReceptionDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);
        await EnsureFournisseurExistsAsync(dto.FournisseurId, cancellationToken);

        var numero = string.IsNullOrWhiteSpace(dto.Numero)
            ? await GenerateNumeroAsync(cancellationToken)
            : dto.Numero.Trim();

        var entity = Mapper.Map<BonReception>(dto with { Numero = numero });
        entity.Note = dto.Note ?? string.Empty;

        await Repo.AddAsync(entity, cancellationToken);
        return (await GetBonReceptionByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateBonReceptionAsync(
        int id,
        UpdateBonReceptionDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureFournisseurExistsAsync(dto.FournisseurId, cancellationToken);
        await ValidateAsync(UpdateValidator, dto, cancellationToken);

        var entity = await Repo.GetByIdWithNavigationsAsync(id, [b => b.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Bon de livraison {id} introuvable.");

        entity.Lignes.Clear();
        Mapper.Map(dto, entity);
        entity.Note = dto.Note ?? string.Empty;

        foreach (var line in entity.Lignes)
        {
            line.Id = 0;
            line.BRId = entity.Id;
        }

        await Repo.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteBonReceptionAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await GetBonReceptionByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Bon de livraison {id} introuvable.");

        await EnsureCanDeleteAsync(id, cancellationToken);
        await DeleteAsync(id, cancellationToken);
    }

    public async Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.Today.Year;
        var prefix = $"BR-{year}-";
        var existing = await Repo.FindAsync(b => b.Numero.StartsWith(prefix), cancellationToken);
        var next = existing
            .Select(b =>
            {
                var tail = b.Numero[prefix.Length..];
                return int.TryParse(tail, out var n) ? n : 0;
            })
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"{prefix}{next:D4}";
    }

    private async Task EnsureFournisseurExistsAsync(int fournisseurId, CancellationToken cancellationToken)
    {
        var fournisseur = await _tiers.GetByIdAsync(fournisseurId, cancellationToken)
            ?? throw new KeyNotFoundException($"Fournisseur {fournisseurId} introuvable.");

        if (fournisseur.Type is not (TypeTiers.Fournisseur or TypeTiers.LesDeux))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(CreateBonReceptionDto.FournisseurId),
                    "Le tiers sélectionné n'est pas un fournisseur.")]);
        }
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        var bl = await Repo.GetByIdAsync(id, cancellationToken);
        if (bl?.FactureFournisseurId is not null)
            linked.Add("facture client");

        if (await _factureLignes.AnyAsync(l => l.BonReceptionId == id, cancellationToken))
            linked.Add("ligne de facture");

        if (linked.Count > 0)
        {
            throw new ValidationException([
                new ValidationFailure(string.Empty,
                    $"Impossible de supprimer ce bon de réception : lié à une {string.Join(", ", linked)}.")]);
        }
    }
}
