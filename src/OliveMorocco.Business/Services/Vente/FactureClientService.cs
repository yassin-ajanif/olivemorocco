using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Common;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Services.Common;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Vente;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Vente;

public sealed class FactureClientService
    : GenericService<FactureClient, FactureClientDto, CreateFactureClientDto, UpdateFactureClientDto>,
      IFactureClientService
{
    private readonly IRepository<FactureClientLigne> _lignes;
    private readonly IRepository<PaiementClient> _paiements;
    private readonly IRepository<AvoirClient> _avoirs;
    private readonly IRepository<Tiers> _tiers;
    private readonly IRepository<BonLivraisonClient> _bonsLivraison;

    public FactureClientService(
        IRepository<FactureClient> factures,
        IRepository<FactureClientLigne> lignes,
        IRepository<PaiementClient> paiements,
        IRepository<AvoirClient> avoirs,
        IRepository<Tiers> tiers,
        IRepository<BonLivraisonClient> bonsLivraison,
        IMapper mapper,
        IEnumerable<IValidator<CreateFactureClientDto>> createValidators,
        IEnumerable<IValidator<UpdateFactureClientDto>> updateValidators)
        : base(factures, mapper, createValidators, updateValidators)
    {
        _lignes = lignes;
        _paiements = paiements;
        _avoirs = avoirs;
        _tiers = tiers;
        _bonsLivraison = bonsLivraison;
    }

    public async Task<PagedResult<FactureClientListItemDto>> GetFacturesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var predicate = (System.Linq.Expressions.Expression<Func<FactureClient, bool>>?)(f =>
            pattern == null
            || EF.Functions.ILike(f.Numero, pattern)
            || EF.Functions.ILike(f.Client.Nom, pattern));

        var (items, totalCount) = await Repo.QueryPagedAsync(
            predicate,
            query => query.OrderByDescending(f => f.Date).ThenByDescending(f => f.Id),
            f => new FactureClientListItemDto(
                f.Id,
                f.Numero,
                f.ClientId,
                f.Client.Nom,
                f.Date,
                f.DateEcheance,
                f.TotalTtc,
                f.EstPayee,
                f.Note),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<FactureClientListItemDto>(items, totalCount);
    }

    public async Task<FactureClientDto?> GetFactureByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdWithNavigationsAsync(
            id,
            [f => f.Client, f => f.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var ligneDtos = await _lignes.FindWithIncludesAsync(
            l => l.FactureClientId == id,
            [l => l.Produit!],
            cancellationToken);

        var paiementRows = await _paiements.FindAsync(
            p => p.FactureClientId == id,
            cancellationToken);

        return new FactureClientDto(
            entity.Id,
            entity.Numero,
            entity.ClientId,
            entity.DevisId,
            entity.Date,
            entity.DateEcheance,
            entity.BonCommandeReference,
            entity.RemiseGlobale,
            entity.TotalTtc,
            entity.EstPayee,
            entity.Note,
            ligneDtos.Select(l => new FactureClientLigneDto(
                l.Id,
                l.FactureClientId,
                l.BonLivraisonId,
                l.ProduitId,
                l.Produit?.Reference ?? string.Empty,
                l.Designation,
                l.Conditionnement,
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA)).ToList(),
            paiementRows
                .OrderByDescending(p => p.Date)
                .ThenByDescending(p => p.Id)
                .Select(p => new FacturePaiementDto(
                    p.Id,
                    p.Date,
                    p.Montant,
                    p.Mode,
                    p.Reference,
                    p.EstEncaisse))
                .ToList());
    }

    public async Task<FactureClientDto> CreateFactureAsync(
        CreateFactureClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);
        await EnsureClientExistsAsync(dto.ClientId, cancellationToken);

        var numero = string.IsNullOrWhiteSpace(dto.Numero)
            ? await GenerateNumeroAsync(cancellationToken)
            : dto.Numero.Trim();

        var (_, _, ttc) = IFactureClientService.ComputeTotals(dto.Lignes, dto.RemiseGlobale);

        var entity = Mapper.Map<FactureClient>(dto with { Numero = numero, TotalTtc = ttc, EstPayee = false });
        entity.Note = dto.Note ?? string.Empty;
        entity.BonCommandeReference = dto.BonCommandeReference ?? string.Empty;
        entity.EstPayee = false;
        NormalizeLines(entity.Lignes);

        await Repo.AddAsync(entity, cancellationToken);
        await LinkBonsLivraisonAsync(entity.Id, dto.ClientId, dto.Lignes, cancellationToken);
        return (await GetFactureByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateFactureAsync(
        int id,
        UpdateFactureClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureClientExistsAsync(dto.ClientId, cancellationToken);
        await ValidateAsync(UpdateValidator, dto, cancellationToken);

        var (_, _, ttc) = IFactureClientService.ComputeTotals(dto.Lignes, dto.RemiseGlobale);
        var paiements = FacturePaiementHelper.Normalize(dto.Paiements);

        var entity = await Repo.GetByIdWithNavigationsAsync(id, [f => f.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Facture {id} introuvable.");

        entity.Lignes.Clear();
        Mapper.Map(dto with { TotalTtc = ttc }, entity);
        entity.Note = dto.Note ?? string.Empty;
        entity.BonCommandeReference = dto.BonCommandeReference ?? string.Empty;
        entity.EstPayee = FacturePaiementHelper.ComputeEstPayee(ttc, paiements);
        NormalizeLines(entity.Lignes);

        foreach (var line in entity.Lignes)
        {
            line.Id = 0;
            line.FactureClientId = entity.Id;
        }

        await Repo.UpdateAsync(entity, cancellationToken);
        await ReplacePaiementsAsync(id, paiements, cancellationToken);
    }

    public async Task DeleteFactureAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await GetFactureByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Facture {id} introuvable.");

        await EnsureCanDeleteAsync(id, cancellationToken);
        await DeleteAsync(id, cancellationToken);
    }

    public async Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.Today.Year;
        var prefix = $"FAC-{year}-";
        var existing = await Repo.FindAsync(f => f.Numero.StartsWith(prefix), cancellationToken);
        var next = existing
            .Select(f =>
            {
                var tail = f.Numero[prefix.Length..];
                return int.TryParse(tail, out var n) ? n : 0;
            })
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"{prefix}{next:D4}";
    }

    private static void NormalizeLines(IEnumerable<FactureClientLigne> lignes)
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
                new ValidationFailure(nameof(CreateFactureClientDto.ClientId),
                    "Le tiers sélectionné n'est pas un client.")]);
        }
    }

    private async Task LinkBonsLivraisonAsync(
        int factureId,
        int clientId,
        IEnumerable<CreateFactureClientLigneDto> lignes,
        CancellationToken cancellationToken)
    {
        var blIds = lignes
            .Where(l => l.BonLivraisonId is > 0)
            .Select(l => l.BonLivraisonId!.Value)
            .Distinct()
            .ToList();

        if (blIds.Count == 0)
            return;

        foreach (var blId in blIds)
        {
            var bl = await _bonsLivraison.GetByIdAsync(blId, cancellationToken)
                ?? throw new KeyNotFoundException($"Bon de livraison {blId} introuvable.");

            if (bl.FactureId is int existingFactureId && existingFactureId != factureId)
            {
                throw new ValidationException([
                    new ValidationFailure(
                        "Lignes",
                        $"Le bon de livraison {bl.Numero} est déjà lié à une autre facture.")]);
            }

            if (bl.ClientId != clientId)
            {
                throw new ValidationException([
                    new ValidationFailure(
                        nameof(CreateFactureClientDto.ClientId),
                        $"Le bon de livraison {bl.Numero} n'appartient pas au même client.")]);
            }

            bl.FactureId = factureId;
            await _bonsLivraison.UpdateAsync(bl, cancellationToken);
        }
    }

    private async Task ReplacePaiementsAsync(
        int factureId,
        IReadOnlyList<CreateFacturePaiementDto> paiements,
        CancellationToken cancellationToken)
    {
        var existing = await _paiements.FindAsync(p => p.FactureClientId == factureId, cancellationToken);
        foreach (var old in existing)
            await _paiements.DeleteAsync(old.Id, cancellationToken);

        foreach (var dto in paiements)
        {
            var entity = new PaiementClient
            {
                FactureClientId = factureId,
                Date = dto.Date,
                Montant = dto.Montant,
                Mode = dto.Mode,
                Reference = dto.Reference?.Trim() ?? string.Empty,
                EstEncaisse = dto.EstEncaisse,
            };

            await _paiements.AddAsync(entity, cancellationToken);
        }
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await _paiements.AnyAsync(p => p.FactureClientId == id, cancellationToken))
            linked.Add("paiement");
        if (await _avoirs.AnyAsync(a => a.FactureId == id, cancellationToken))
            linked.Add("avoir");

        if (linked.Count > 0)
        {
            throw new ValidationException([
                new ValidationFailure(string.Empty,
                    $"Impossible de supprimer cette facture : liée à un {string.Join(", ", linked)}.")]);
        }
    }
}
