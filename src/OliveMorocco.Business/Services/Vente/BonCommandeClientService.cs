using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Vente;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Vente;

public sealed class BonCommandeClientService
    : GenericService<BonCommandeClient, BonCommandeClientDto, CreateBonCommandeClientDto, UpdateBonCommandeClientDto>,
      IBonCommandeClientService
{
    private readonly IRepository<BonCommandeClientLigne> _lignes;
    private readonly IRepository<Tiers> _tiers;
    private readonly IRepository<BonLivraisonClient> _bonsLivraison;

    public BonCommandeClientService(
        IRepository<BonCommandeClient> bons,
        IRepository<BonCommandeClientLigne> lignes,
        IRepository<Tiers> tiers,
        IRepository<BonLivraisonClient> bonsLivraison,
        IMapper mapper,
        IEnumerable<IValidator<CreateBonCommandeClientDto>> createValidators,
        IEnumerable<IValidator<UpdateBonCommandeClientDto>> updateValidators)
        : base(bons, mapper, createValidators, updateValidators)
    {
        _lignes = lignes;
        _tiers = tiers;
        _bonsLivraison = bonsLivraison;
    }

    public async Task<PagedResult<BonCommandeClientListItemDto>> GetBonsCommandeAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var predicate = (System.Linq.Expressions.Expression<Func<BonCommandeClient, bool>>?)(b =>
            pattern == null
            || EF.Functions.ILike(b.Numero, pattern)
            || EF.Functions.ILike(b.Client.Nom, pattern));

        var (items, totalCount) = await Repo.QueryPagedAsync(
            predicate,
            query => query.OrderByDescending(b => b.Date).ThenByDescending(b => b.Id),
            b => b,
            page,
            pageSize,
            cancellationToken);

        var ids = items.Select(b => b.Id).ToList();
        if (ids.Count == 0)
            return new PagedResult<BonCommandeClientListItemDto>([], totalCount);

        var headers = await Repo.FindWithIncludesAsync(
            b => ids.Contains(b.Id),
            [b => b.Client, b => b.Lignes],
            cancellationToken);

        var byId = headers.ToDictionary(b => b.Id);
        var listItems = items
            .Where(b => byId.ContainsKey(b.Id))
            .Select(b =>
            {
                var full = byId[b.Id];
                var (_, _, ttc) = IBonCommandeClientService.ComputeTotals(
                    full.Lignes.Select(l => new CreateBonCommandeClientLigneDto(
                        l.ProduitId,
                        l.Designation,
                        l.QuantiteCommandee,
                        l.PrixUnitaireHT,
                        l.Remise,
                        l.TauxTVA,
                        l.Conditionnement)));

                return new BonCommandeClientListItemDto(
                    full.Id,
                    full.Numero,
                    full.ClientId,
                    full.Client.Nom,
                    full.Date,
                    ttc,
                    full.Note);
            })
            .ToList();

        return new PagedResult<BonCommandeClientListItemDto>(listItems, totalCount);
    }

    public async Task<BonCommandeClientDto?> GetBonCommandeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdWithNavigationsAsync(
            id,
            [b => b.Client, b => b.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var ligneDtos = await _lignes.FindWithIncludesAsync(
            l => l.BonCommandeClientId == id,
            [l => l.Produit!],
            cancellationToken);

        var (_, _, ttc) = IBonCommandeClientService.ComputeTotals(
            ligneDtos.Select(l => new CreateBonCommandeClientLigneDto(
                l.ProduitId,
                l.Designation,
                l.QuantiteCommandee,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA,
                l.Conditionnement)));

        return new BonCommandeClientDto(
            entity.Id,
            entity.Numero,
            entity.ClientId,
            entity.DevisId,
            entity.FactureId,
            entity.Date,
            ttc,
            entity.Note,
            ligneDtos.Select(l => new BonCommandeClientLigneDto(
                l.Id,
                l.BonCommandeClientId,
                l.ProduitId,
                l.Produit?.Reference ?? string.Empty,
                l.Designation,
                l.Conditionnement,
                l.QuantiteCommandee,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA)).ToList());
    }

    public async Task<BonCommandeClientDto> CreateBonCommandeAsync(
        CreateBonCommandeClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);
        await EnsureClientExistsAsync(dto.ClientId, cancellationToken);

        var numero = string.IsNullOrWhiteSpace(dto.Numero)
            ? await GenerateNumeroAsync(cancellationToken)
            : dto.Numero.Trim();

        var entity = Mapper.Map<BonCommandeClient>(dto with { Numero = numero });
        entity.Note = dto.Note ?? string.Empty;
        NormalizeLines(entity.Lignes);

        await Repo.AddAsync(entity, cancellationToken);
        return (await GetBonCommandeByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateBonCommandeAsync(
        int id,
        UpdateBonCommandeClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureClientExistsAsync(dto.ClientId, cancellationToken);
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
            line.BonCommandeClientId = entity.Id;
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

    private static void NormalizeLines(IEnumerable<BonCommandeClientLigne> lignes)
    {
        foreach (var line in lignes)
            line.Conditionnement = string.IsNullOrWhiteSpace(line.Conditionnement) ? string.Empty : line.Conditionnement.Trim();
    }

    private async Task EnsureClientExistsAsync(int clientId, CancellationToken cancellationToken)
    {
        var client = await _tiers.GetByIdAsync(clientId, cancellationToken)
            ?? throw new KeyNotFoundException($"Client {clientId} introuvable.");

        if (client.Type is not (TypeTiers.Client or TypeTiers.LesDeux))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(CreateBonCommandeClientDto.ClientId),
                    "Le tiers sélectionné n'est pas un client.")]);
        }
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await _bonsLivraison.AnyAsync(b => b.BonCommandeClientId == id, cancellationToken))
            linked.Add("bon de livraison");

        var bon = await Repo.GetByIdAsync(id, cancellationToken);
        if (bon?.FactureId is not null)
            linked.Add("facture client");

        if (linked.Count > 0)
        {
            throw new ValidationException([
                new ValidationFailure(string.Empty,
                    $"Impossible de supprimer ce bon de commande : lié à un {string.Join(", ", linked)}.")]);
        }
    }
}
