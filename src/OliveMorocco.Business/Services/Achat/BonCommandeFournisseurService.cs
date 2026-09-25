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

public sealed class BonCommandeFournisseurService
    : GenericService<BonCommandeFournisseur, BonCommandeFournisseurDto, CreateBonCommandeFournisseurDto, UpdateBonCommandeFournisseurDto>,
      IBonCommandeFournisseurService
{
    private readonly IRepository<BonCommandeFournisseurLigne> _lignes;
    private readonly IRepository<Tiers> _tiers;
    private readonly IRepository<BonReception> _bonsReception;

    public BonCommandeFournisseurService(
        IRepository<BonCommandeFournisseur> bons,
        IRepository<BonCommandeFournisseurLigne> lignes,
        IRepository<Tiers> tiers,
        IRepository<BonReception> bonsLivraison,
        IMapper mapper,
        IEnumerable<IValidator<CreateBonCommandeFournisseurDto>> createValidators,
        IEnumerable<IValidator<UpdateBonCommandeFournisseurDto>> updateValidators)
        : base(bons, mapper, createValidators, updateValidators)
    {
        _lignes = lignes;
        _tiers = tiers;
        _bonsReception = bonsLivraison;
    }

    public async Task<PagedResult<BonCommandeFournisseurListItemDto>> GetBonsCommandeAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var predicate = (System.Linq.Expressions.Expression<Func<BonCommandeFournisseur, bool>>?)(b =>
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
            return new PagedResult<BonCommandeFournisseurListItemDto>([], totalCount);

        var headers = await Repo.FindWithIncludesAsync(
            b => ids.Contains(b.Id),
            [b => b.Fournisseur, b => b.Lignes],
            cancellationToken);

        var byId = headers.ToDictionary(b => b.Id);
        var listItems = items
            .Where(b => byId.ContainsKey(b.Id))
            .Select(b =>
            {
                var full = byId[b.Id];
                var (_, _, ttc) = IBonCommandeFournisseurService.ComputeTotals(
                    full.Lignes.Select(l => new CreateBonCommandeFournisseurLigneDto(
                        l.ProduitId,
                        l.ServiceId,
                        l.Designation,
                        l.QuantiteCommandee,
                        l.PrixUnitaireHT,
                        l.Remise,
                        l.TauxTVA,
                        l.Conditionnement)));

                return new BonCommandeFournisseurListItemDto(
                    full.Id,
                    full.Numero,
                    full.FournisseurId,
                    full.Fournisseur.Nom,
                    full.Date,
                    ttc,
                    full.Note);
            })
            .ToList();

        return new PagedResult<BonCommandeFournisseurListItemDto>(listItems, totalCount);
    }

    public async Task<BonCommandeFournisseurDto?> GetBonCommandeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdWithNavigationsAsync(
            id,
            [b => b.Fournisseur, b => b.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var ligneDtos = await _lignes.FindWithIncludesAsync(
            l => l.BonCommandeFournisseurId == id,
            [l => l.Produit!],
            cancellationToken);

        var (_, _, ttc) = IBonCommandeFournisseurService.ComputeTotals(
            ligneDtos.Select(l => new CreateBonCommandeFournisseurLigneDto(
                l.ProduitId,
                l.ServiceId,
                l.Designation,
                l.QuantiteCommandee,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA,
                l.Conditionnement)));

        return new BonCommandeFournisseurDto(
            entity.Id,
            entity.Numero,
            entity.FournisseurId,
            entity.Date,
            ttc,
            entity.Note,
            ligneDtos.Select(l => new BonCommandeFournisseurLigneDto(
                l.Id,
                l.BonCommandeFournisseurId,
                l.ProduitId,
                l.ServiceId,
                l.Produit?.Reference ?? string.Empty,
                l.Designation,
                l.Conditionnement,
                l.QuantiteCommandee,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA)).ToList());
    }

    public async Task<BonCommandeFournisseurDto> CreateBonCommandeAsync(
        CreateBonCommandeFournisseurDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);
        await EnsureFournisseurExistsAsync(dto.FournisseurId, cancellationToken);

        var numero = string.IsNullOrWhiteSpace(dto.Numero)
            ? await GenerateNumeroAsync(cancellationToken)
            : dto.Numero.Trim();

        var entity = Mapper.Map<BonCommandeFournisseur>(dto with { Numero = numero });
        entity.Note = dto.Note ?? string.Empty;
        NormalizeLines(entity.Lignes);

        await Repo.AddAsync(entity, cancellationToken);
        return (await GetBonCommandeByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateBonCommandeAsync(
        int id,
        UpdateBonCommandeFournisseurDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureFournisseurExistsAsync(dto.FournisseurId, cancellationToken);
        await ValidateAsync(UpdateValidator, dto, cancellationToken);

        var entity = await Repo.GetByIdWithNavigationsAsync(id, [b => b.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Bon de commande {id} introuvable.");

        entity.Lignes.Clear();
        Mapper.Map(dto, entity);
        entity.Note = dto.Note ?? string.Empty;
        NormalizeLines(entity.Lignes);

        foreach (var line in entity.Lignes)
        {
            line.Id = 0;
            line.BonCommandeFournisseurId = entity.Id;
        }

        await Repo.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteBonCommandeAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await GetBonCommandeByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Bon de commande {id} introuvable.");

        await EnsureCanDeleteAsync(id, cancellationToken);
        await DeleteAsync(id, cancellationToken);
    }

    public async Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.Today.Year;
        var prefix = $"BC-{year}-";
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

    private static void NormalizeLines(IEnumerable<BonCommandeFournisseurLigne> lignes)
    {
        foreach (var line in lignes)
            line.Conditionnement = string.IsNullOrWhiteSpace(line.Conditionnement) ? string.Empty : line.Conditionnement.Trim();
    }

    private async Task EnsureFournisseurExistsAsync(int fournisseurId, CancellationToken cancellationToken)
    {
        var fournisseur = await _tiers.GetByIdAsync(fournisseurId, cancellationToken)
            ?? throw new KeyNotFoundException($"Fournisseur {fournisseurId} introuvable.");

        if (fournisseur.Type is not (TypeTiers.Fournisseur or TypeTiers.LesDeux))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(CreateBonCommandeFournisseurDto.FournisseurId),
                    "Le tiers sélectionné n'est pas un fournisseur.")]);
        }
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await _bonsReception.AnyAsync(b => b.BonCommandeId == id, cancellationToken))
            linked.Add("bon de réception");
        if (linked.Count > 0)
        {
            throw new ValidationException([
                new ValidationFailure(string.Empty,
                    $"Impossible de supprimer ce bon de commande : lié à un {string.Join(", ", linked)}.")]);
        }
    }
}
