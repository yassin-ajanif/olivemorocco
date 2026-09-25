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

public sealed class DevisClientService
    : GenericService<DevisClient, DevisClientDto, CreateDevisClientDto, UpdateDevisClientDto>,
      IDevisClientService
{
    private readonly IRepository<DevisClientLigne> _lignes;
    private readonly IRepository<Tiers> _tiers;
    private readonly IRepository<BonCommandeClient> _bonsCommande;
    private readonly IRepository<BonLivraisonClient> _bonsLivraison;
    private readonly IRepository<FactureClient> _factures;

    public DevisClientService(
        IRepository<DevisClient> devis,
        IRepository<DevisClientLigne> lignes,
        IRepository<Tiers> tiers,
        IRepository<BonCommandeClient> bonsCommande,
        IRepository<BonLivraisonClient> bonsLivraison,
        IRepository<FactureClient> factures,
        IMapper mapper,
        IEnumerable<IValidator<CreateDevisClientDto>> createValidators,
        IEnumerable<IValidator<UpdateDevisClientDto>> updateValidators)
        : base(devis, mapper, createValidators, updateValidators)
    {
        _lignes = lignes;
        _tiers = tiers;
        _bonsCommande = bonsCommande;
        _bonsLivraison = bonsLivraison;
        _factures = factures;
    }

    public async Task<PagedResult<DevisClientListItemDto>> GetDevisAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var predicate = (System.Linq.Expressions.Expression<Func<DevisClient, bool>>?)(d =>
            pattern == null
            || EF.Functions.ILike(d.Numero, pattern)
            || EF.Functions.ILike(d.Client.Nom, pattern));

        var (items, totalCount) = await Repo.QueryPagedAsync(
            predicate,
            query => query.OrderByDescending(d => d.Date).ThenByDescending(d => d.Id),
            d => d,
            page,
            pageSize,
            cancellationToken);

        var ids = items.Select(d => d.Id).ToList();
        if (ids.Count == 0)
            return new PagedResult<DevisClientListItemDto>([], totalCount);

        var headers = await Repo.FindWithIncludesAsync(
            d => ids.Contains(d.Id),
            [d => d.Client, d => d.Lignes],
            cancellationToken);

        var byId = headers.ToDictionary(d => d.Id);
        var listItems = items
            .Where(d => byId.ContainsKey(d.Id))
            .Select(d =>
            {
                var full = byId[d.Id];
                var (_, _, ttc) = IDevisClientService.ComputeTotals(
                    full.Lignes.Select(l => new CreateDevisClientLigneDto(
                        l.ProduitId,
                        l.Designation,
                        l.Quantite,
                        l.PrixUnitaireHT,
                        l.Remise,
                        l.TauxTVA,
                        l.Conditionnement)),
                    full.RemiseGlobale);

                return new DevisClientListItemDto(
                    full.Id,
                    full.Numero,
                    full.ClientId,
                    full.Client.Nom,
                    full.Date,
                    full.DateValidite,
                    ttc,
                    full.Note);
            })
            .ToList();

        return new PagedResult<DevisClientListItemDto>(listItems, totalCount);
    }

    public async Task<DevisClientDto?> GetDevisByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdWithNavigationsAsync(
            id,
            [d => d.Client, d => d.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var ligneDtos = await _lignes.FindWithIncludesAsync(
            l => l.DevisClientId == id,
            [l => l.Produit!],
            cancellationToken);

        var (_, _, ttc) = IDevisClientService.ComputeTotals(
            ligneDtos.Select(l => new CreateDevisClientLigneDto(
                l.ProduitId,
                l.Designation,
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA,
                l.Conditionnement)),
            entity.RemiseGlobale);

        return new DevisClientDto(
            entity.Id,
            entity.Numero,
            entity.ClientId,
            entity.Date,
            entity.DateValidite,
            entity.RemiseGlobale,
            entity.Note,
            ttc,
            ligneDtos.Select(l => new DevisClientLigneDto(
                l.Id,
                l.DevisClientId,
                l.ProduitId,
                l.Produit?.Reference ?? string.Empty,
                l.Designation,
                l.Conditionnement,
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA)).ToList());
    }

    public async Task<DevisClientDto> CreateDevisAsync(
        CreateDevisClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);
        await EnsureClientExistsAsync(dto.ClientId, cancellationToken);

        var numero = string.IsNullOrWhiteSpace(dto.Numero)
            ? await GenerateNumeroAsync(cancellationToken)
            : dto.Numero.Trim();

        var entity = Mapper.Map<DevisClient>(dto with { Numero = numero });
        entity.Note = dto.Note ?? string.Empty;

        foreach (var line in entity.Lignes)
            line.Conditionnement = string.IsNullOrWhiteSpace(line.Conditionnement) ? string.Empty : line.Conditionnement.Trim();

        await Repo.AddAsync(entity, cancellationToken);
        return (await GetDevisByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateDevisAsync(
        int id,
        UpdateDevisClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureClientExistsAsync(dto.ClientId, cancellationToken);
        await ValidateAsync(UpdateValidator, dto, cancellationToken);

        var entity = await Repo.GetByIdWithNavigationsAsync(id, [d => d.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Devis {id} introuvable.");

        entity.Lignes.Clear();
        Mapper.Map(dto, entity);
        entity.Note = dto.Note ?? string.Empty;

        foreach (var line in entity.Lignes)
        {
            line.Id = 0;
            line.DevisClientId = entity.Id;
            line.Conditionnement = string.IsNullOrWhiteSpace(line.Conditionnement) ? string.Empty : line.Conditionnement.Trim();
        }

        await Repo.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteDevisAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await GetDevisByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Devis {id} introuvable.");

        await EnsureCanDeleteAsync(id, cancellationToken);
        await DeleteAsync(id, cancellationToken);
    }

    public async Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.Today.Year;
        var prefix = $"DEV-{year}-";
        var existing = await Repo.FindAsync(d => d.Numero.StartsWith(prefix), cancellationToken);
        var next = existing
            .Select(d =>
            {
                var tail = d.Numero[prefix.Length..];
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
            throw new ValidationException([
                new ValidationFailure(nameof(CreateDevisClientDto.ClientId),
                    "Le tiers sélectionné n'est pas un client.")]);
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await _bonsCommande.AnyAsync(b => b.DevisId == id, cancellationToken))
            linked.Add("bon de commande client");
        if (await _bonsLivraison.AnyAsync(b => b.DevisId == id, cancellationToken))
            linked.Add("bon de livraison");
        if (await _factures.AnyAsync(f => f.DevisId == id, cancellationToken))
            linked.Add("facture client");

        if (linked.Count > 0)
        {
            throw new ValidationException([
                new ValidationFailure(string.Empty,
                    $"Impossible de supprimer ce devis : lié à un {string.Join(", ", linked)}.")]);
        }
    }
}
