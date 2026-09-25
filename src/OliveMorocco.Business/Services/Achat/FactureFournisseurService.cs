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

public sealed class FactureFournisseurService
    : GenericService<FactureFournisseur, FactureFournisseurDto, CreateFactureFournisseurDto, UpdateFactureFournisseurDto>,
      IFactureFournisseurService
{
    private readonly IRepository<FactureFournisseurLigne> _lignes;
    private readonly IRepository<PaiementFournisseur> _paiements;
    private readonly IRepository<AvoirFournisseur> _avoirs;
    private readonly IRepository<Tiers> _tiers;
    private readonly IRepository<BonReception> _bonsReception;

    public FactureFournisseurService(
        IRepository<FactureFournisseur> factures,
        IRepository<FactureFournisseurLigne> lignes,
        IRepository<PaiementFournisseur> paiements,
        IRepository<AvoirFournisseur> avoirs,
        IRepository<Tiers> tiers,
        IRepository<BonReception> bonsReception,
        IMapper mapper,
        IEnumerable<IValidator<CreateFactureFournisseurDto>> createValidators,
        IEnumerable<IValidator<UpdateFactureFournisseurDto>> updateValidators)
        : base(factures, mapper, createValidators, updateValidators)
    {
        _lignes = lignes;
        _paiements = paiements;
        _avoirs = avoirs;
        _tiers = tiers;
        _bonsReception = bonsReception;
    }

    public async Task<PagedResult<FactureFournisseurListItemDto>> GetFacturesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var predicate = (System.Linq.Expressions.Expression<Func<FactureFournisseur, bool>>?)(f =>
            pattern == null
            || EF.Functions.ILike(f.Numero, pattern)
            || EF.Functions.ILike(f.Fournisseur.Nom, pattern));

        var (items, totalCount) = await Repo.QueryPagedAsync(
            predicate,
            query => query.OrderByDescending(f => f.Date).ThenByDescending(f => f.Id),
            f => new FactureFournisseurListItemDto(
                f.Id,
                f.Numero,
                f.FournisseurId,
                f.Fournisseur.Nom,
                f.Date,
                f.DateEcheance,
                f.TotalTtc,
                f.EstPayee,
                f.Note),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<FactureFournisseurListItemDto>(items, totalCount);
    }

    public async Task<FactureFournisseurDto?> GetFactureByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdWithNavigationsAsync(
            id,
            [f => f.Fournisseur, f => f.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var ligneDtos = await _lignes.FindWithIncludesAsync(
            l => l.FactureFournisseurId == id,
            [l => l.Intrant!],
            cancellationToken);

        return new FactureFournisseurDto(
            entity.Id,
            entity.Numero,
            entity.FournisseurId,
            entity.Date,
            entity.DateEcheance,
            entity.RemiseGlobale,
            entity.TotalTtc,
            entity.EstPayee,
            entity.Note,
            ligneDtos.Select(l => new FactureFournisseurLigneDto(
                l.Id,
                l.FactureFournisseurId,
                l.BonReceptionId,
                l.IntrantId,
                l.ServiceId,
                string.Empty,
                l.Designation,
                l.Conditionnement,
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA)).ToList());
    }

    public async Task<FactureFournisseurDto> CreateFactureAsync(
        CreateFactureFournisseurDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);
        await EnsureFournisseurExistsAsync(dto.FournisseurId, cancellationToken);

        var numero = string.IsNullOrWhiteSpace(dto.Numero)
            ? await GenerateNumeroAsync(cancellationToken)
            : dto.Numero.Trim();

        var (_, _, ttc) = IFactureFournisseurService.ComputeTotals(dto.Lignes, dto.RemiseGlobale);

        var entity = Mapper.Map<FactureFournisseur>(dto with { Numero = numero, TotalTtc = ttc });
        entity.Note = dto.Note ?? string.Empty;
        NormalizeLines(entity.Lignes);

        await Repo.AddAsync(entity, cancellationToken);
        await LinkBonsReceptionAsync(entity.Id, dto.FournisseurId, dto.Lignes, cancellationToken);
        return (await GetFactureByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateFactureAsync(
        int id,
        UpdateFactureFournisseurDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureFournisseurExistsAsync(dto.FournisseurId, cancellationToken);
        await ValidateAsync(UpdateValidator, dto, cancellationToken);

        var (_, _, ttc) = IFactureFournisseurService.ComputeTotals(dto.Lignes, dto.RemiseGlobale);

        var entity = await Repo.GetByIdWithNavigationsAsync(id, [f => f.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Facture {id} introuvable.");

        entity.Lignes.Clear();
        Mapper.Map(dto with { TotalTtc = ttc }, entity);
        entity.Note = dto.Note ?? string.Empty;
        NormalizeLines(entity.Lignes);

        foreach (var line in entity.Lignes)
        {
            line.Id = 0;
            line.FactureFournisseurId = entity.Id;
        }

        await Repo.UpdateAsync(entity, cancellationToken);
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
        var prefix = $"FF-{year}-";
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

    private static void NormalizeLines(IEnumerable<FactureFournisseurLigne> lignes)
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
                new ValidationFailure(nameof(CreateFactureFournisseurDto.FournisseurId),
                    "Le tiers sélectionné n'est pas un fournisseur.")]);
        }
    }

    private async Task LinkBonsReceptionAsync(
        int factureId,
        int clientId,
        IEnumerable<CreateFactureFournisseurLigneDto> lignes,
        CancellationToken cancellationToken)
    {
        var blIds = lignes
            .Where(l => l.BonReceptionId is > 0)
            .Select(l => l.BonReceptionId!.Value)
            .Distinct()
            .ToList();

        if (blIds.Count == 0)
            return;

        foreach (var blId in blIds)
        {
            var bl = await _bonsReception.GetByIdAsync(blId, cancellationToken)
                ?? throw new KeyNotFoundException($"Bon de réception {blId} introuvable.");

            if (bl.FactureFournisseurId is int existingFactureId && existingFactureId != factureId)
            {
                throw new ValidationException([
                    new ValidationFailure(
                        "Lignes",
                        $"Le bon de réception {bl.Numero} est déjà lié à une autre facture.")]);
            }

            if (bl.FournisseurId != clientId)
            {
                throw new ValidationException([
                    new ValidationFailure(
                        nameof(CreateFactureFournisseurDto.FournisseurId),
                        $"Le bon de réception {bl.Numero} n'appartient pas au même fournisseur.")]);
            }

            bl.FactureFournisseurId = factureId;
            await _bonsReception.UpdateAsync(bl, cancellationToken);
        }
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await _paiements.AnyAsync(p => p.FactureFournisseurId == id, cancellationToken))
            linked.Add("paiement");

        if (linked.Count > 0)
        {
            throw new ValidationException([
                new ValidationFailure(string.Empty,
                    $"Impossible de supprimer cette facture : liée à un {string.Join(", ", linked)}.")]);
        }
    }
}
