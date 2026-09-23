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

public sealed class BonLivraisonClientService
    : GenericService<BonLivraisonClient, BonLivraisonClientDto, CreateBonLivraisonClientDto, UpdateBonLivraisonClientDto>,
      IBonLivraisonClientService
{
    private readonly IRepository<BonLivraisonClientLigne> _lignes;
    private readonly IRepository<Tiers> _tiers;
    private readonly IRepository<FactureClientLigne> _factureLignes;
    private readonly IRepository<FactureClient> _factures;

    public BonLivraisonClientService(
        IRepository<BonLivraisonClient> bons,
        IRepository<BonLivraisonClientLigne> lignes,
        IRepository<Tiers> tiers,
        IRepository<FactureClientLigne> factureLignes,
        IRepository<FactureClient> factures,
        IMapper mapper,
        IEnumerable<IValidator<CreateBonLivraisonClientDto>> createValidators,
        IEnumerable<IValidator<UpdateBonLivraisonClientDto>> updateValidators)
        : base(bons, mapper, createValidators, updateValidators)
    {
        _lignes = lignes;
        _tiers = tiers;
        _factureLignes = factureLignes;
        _factures = factures;
    }

    public async Task<PagedResult<BonLivraisonClientListItemDto>> GetBonsLivraisonAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var predicate = (System.Linq.Expressions.Expression<Func<BonLivraisonClient, bool>>?)(b =>
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
            return new PagedResult<BonLivraisonClientListItemDto>([], totalCount);

        var headers = await Repo.FindWithIncludesAsync(
            b => ids.Contains(b.Id),
            [b => b.Client, b => b.Lignes],
            cancellationToken);

        var byId = headers.ToDictionary(b => b.Id);
        var factureIds = headers
            .Where(b => b.FactureId is not null)
            .Select(b => b.FactureId!.Value)
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
                var (_, _, ttc) = IBonLivraisonClientService.ComputeTotals(
                    full.Lignes.Select(l => new CreateBonLivraisonClientLigneDto(
                        l.ProduitId,
                        l.Designation,
                        l.QuantiteCommandee,
                        l.QuantiteLivree,
                        l.PrixUnitaireHT,
                        l.Remise,
                        l.TauxTVA)));

                string? factureNumero = null;
                if (full.FactureId is int factureId && factureNumeros.TryGetValue(factureId, out var numero))
                    factureNumero = numero;

                return new BonLivraisonClientListItemDto(
                    full.Id,
                    full.Numero,
                    full.ClientId,
                    full.Client.Nom,
                    full.Date,
                    ttc,
                    full.Note,
                    full.FactureId,
                    factureNumero);
            })
            .ToList();

        return new PagedResult<BonLivraisonClientListItemDto>(listItems, totalCount);
    }

    public async Task<BonLivraisonClientDto?> GetBonLivraisonByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdWithNavigationsAsync(
            id,
            [b => b.Client, b => b.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var ligneDtos = await _lignes.FindWithIncludesAsync(
            l => l.BLId == id,
            [l => l.Produit!],
            cancellationToken);

        var (_, _, ttc) = IBonLivraisonClientService.ComputeTotals(
            ligneDtos.Select(l => new CreateBonLivraisonClientLigneDto(
                l.ProduitId,
                l.Designation,
                l.QuantiteCommandee,
                l.QuantiteLivree,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA)));

        string? factureNumero = null;
        if (entity.FactureId is int factureId)
        {
            var facture = await _factures.GetByIdAsync(factureId, cancellationToken);
            factureNumero = facture?.Numero;
        }

        return new BonLivraisonClientDto(
            entity.Id,
            entity.Numero,
            entity.ClientId,
            entity.DevisId,
            entity.BonCommandeClientId,
            entity.FactureId,
            factureNumero,
            entity.Date,
            ttc,
            entity.Note,
            ligneDtos.Select(l => new BonLivraisonClientLigneDto(
                l.Id,
                l.BLId,
                l.ProduitId,
                l.Produit?.Reference ?? string.Empty,
                l.Designation,
                l.QuantiteCommandee,
                l.QuantiteLivree,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA)).ToList());
    }

    public async Task<BonLivraisonClientDto> CreateBonLivraisonAsync(
        CreateBonLivraisonClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);
        await EnsureClientExistsAsync(dto.ClientId, cancellationToken);

        var numero = string.IsNullOrWhiteSpace(dto.Numero)
            ? await GenerateNumeroAsync(cancellationToken)
            : dto.Numero.Trim();

        var entity = Mapper.Map<BonLivraisonClient>(dto with { Numero = numero });
        entity.Note = dto.Note ?? string.Empty;

        await Repo.AddAsync(entity, cancellationToken);
        return (await GetBonLivraisonByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateBonLivraisonAsync(
        int id,
        UpdateBonLivraisonClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureClientExistsAsync(dto.ClientId, cancellationToken);
        await ValidateAsync(UpdateValidator, dto, cancellationToken);

        var entity = await Repo.GetByIdWithNavigationsAsync(id, [b => b.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Bon de livraison {id} introuvable.");

        entity.Lignes.Clear();
        Mapper.Map(dto, entity);
        entity.Note = dto.Note ?? string.Empty;

        foreach (var line in entity.Lignes)
        {
            line.Id = 0;
            line.BLId = entity.Id;
        }

        await Repo.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteBonLivraisonAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await GetBonLivraisonByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Bon de livraison {id} introuvable.");

        await EnsureCanDeleteAsync(id, cancellationToken);
        await DeleteAsync(id, cancellationToken);
    }

    public async Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.Today.Year;
        var prefix = $"BL-{year}-";
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

    private async Task EnsureClientExistsAsync(int clientId, CancellationToken cancellationToken)
    {
        var client = await _tiers.GetByIdAsync(clientId, cancellationToken)
            ?? throw new KeyNotFoundException($"Client {clientId} introuvable.");

        if (client.Type is not (TypeTiers.Client or TypeTiers.LesDeux))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(CreateBonLivraisonClientDto.ClientId),
                    "Le tiers sélectionné n'est pas un client.")]);
        }
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        var bl = await Repo.GetByIdAsync(id, cancellationToken);
        if (bl?.FactureId is not null)
            linked.Add("facture client");

        if (await _factureLignes.AnyAsync(l => l.BonLivraisonId == id, cancellationToken))
            linked.Add("ligne de facture");

        if (linked.Count > 0)
        {
            throw new ValidationException([
                new ValidationFailure(string.Empty,
                    $"Impossible de supprimer ce bon de livraison : lié à une {string.Join(", ", linked)}.")]);
        }
    }
}
