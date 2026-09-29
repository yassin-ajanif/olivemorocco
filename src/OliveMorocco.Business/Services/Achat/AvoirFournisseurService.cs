using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.Services.Stockage.Intrants;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Achat;

public sealed class AvoirFournisseurService
    : GenericService<AvoirFournisseur, AvoirFournisseurDto, CreateAvoirFournisseurDto, UpdateAvoirFournisseurDto>,
      IAvoirFournisseurService
{
    private readonly IRepository<AvoirFournisseurLigne> _lignes;
    private readonly IRepository<Tiers> _tiers;
    private readonly IStockIntrantService _stockIntrantService;

    public AvoirFournisseurService(
        IRepository<AvoirFournisseur> avoirs,
        IRepository<AvoirFournisseurLigne> lignes,
        IRepository<Tiers> tiers,
        IStockIntrantService stockIntrantService,
        IMapper mapper,
        IEnumerable<IValidator<CreateAvoirFournisseurDto>> createValidators,
        IEnumerable<IValidator<UpdateAvoirFournisseurDto>> updateValidators)
        : base(avoirs, mapper, createValidators, updateValidators)
    {
        _lignes = lignes;
        _tiers = tiers;
        _stockIntrantService = stockIntrantService;
    }

    public async Task<PagedResult<AvoirFournisseurListItemDto>> GetAvoirsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var predicate = (System.Linq.Expressions.Expression<Func<AvoirFournisseur, bool>>?)(a =>
            pattern == null
            || EF.Functions.ILike(a.Numero, pattern)
            || EF.Functions.ILike(a.Fournisseur.Nom, pattern)
            || EF.Functions.ILike(a.Motif, pattern));

        var (items, totalCount) = await Repo.QueryPagedAsync(
            predicate,
            query => query.OrderByDescending(a => a.Date).ThenByDescending(a => a.Id),
            a => a,
            page,
            pageSize,
            cancellationToken);

        var ids = items.Select(a => a.Id).ToList();
        if (ids.Count == 0)
            return new PagedResult<AvoirFournisseurListItemDto>([], totalCount);

        var headers = await Repo.FindWithIncludesAsync(
            a => ids.Contains(a.Id),
            [a => a.Fournisseur, a => a.Lignes],
            cancellationToken);

        var byId = headers.ToDictionary(a => a.Id);
        var listItems = items
            .Where(a => byId.ContainsKey(a.Id))
            .Select(a =>
            {
                var full = byId[a.Id];
                var (_, _, ttc) = IAvoirFournisseurService.ComputeTotals(
                    full.Lignes.Select(l => new CreateAvoirFournisseurLigneDto(
                        l.IntrantId,
                        l.Designation,
                        l.Quantite,
                        l.PrixUnitaireHT,
                        l.Remise,
                        l.TauxTVA,
                        l.Conditionnement)));

                return new AvoirFournisseurListItemDto(
                    full.Id,
                    full.Numero,
                    full.FournisseurId,
                    full.Fournisseur.Nom,
                    full.Date,
                    ttc,
                    full.Motif,
                    full.RetourMarchandise);
            })
            .ToList();

        return new PagedResult<AvoirFournisseurListItemDto>(listItems, totalCount);
    }

    public async Task<AvoirFournisseurDto?> GetAvoirByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdWithNavigationsAsync(
            id,
            [a => a.Fournisseur, a => a.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var ligneDtos = await _lignes.FindWithIncludesAsync(
            l => l.AvoirFournisseurId == id,
            [l => l.Intrant!],
            cancellationToken);

        var (_, _, ttc) = IAvoirFournisseurService.ComputeTotals(
            ligneDtos.Select(l => new CreateAvoirFournisseurLigneDto(
                l.IntrantId,
                l.Designation,
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA,
                l.Conditionnement)));

        return new AvoirFournisseurDto(
            entity.Id,
            entity.Numero,
            entity.FournisseurId,
            entity.Date,
            entity.Motif,
            entity.RetourMarchandise,
            ttc,
            ligneDtos.Select(l => new AvoirFournisseurLigneDto(
                l.Id,
                l.AvoirFournisseurId,
                l.IntrantId,
                AchatLineReference.FromIntrant(l.Intrant),
                l.Designation,
                l.Conditionnement,
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA)).ToList());
    }

    public async Task<AvoirFournisseurDto> CreateAvoirAsync(
        CreateAvoirFournisseurDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);
        await EnsureFournisseurExistsAsync(dto.FournisseurId, cancellationToken);

        var numero = string.IsNullOrWhiteSpace(dto.Numero)
            ? await GenerateNumeroAsync(cancellationToken)
            : dto.Numero.Trim();

        var entity = Mapper.Map<AvoirFournisseur>(dto with { Numero = numero });
        entity.Motif = dto.Motif ?? string.Empty;
        NormalizeLines(entity.Lignes);

        await Repo.ExecuteInTransactionAsync(async ct =>
        {
            await Repo.AddAsync(entity, ct);
            await _stockIntrantService.ApplyAvoirFournisseurSortieAsync(
                entity.Id,
                entity.RetourMarchandise,
                entity.Lignes.Select(l => (l.IntrantId, l.Quantite)),
                ct);
        }, cancellationToken);

        return (await GetAvoirByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateAvoirAsync(
        int id,
        UpdateAvoirFournisseurDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureFournisseurExistsAsync(dto.FournisseurId, cancellationToken);
        await ValidateAsync(UpdateValidator, dto, cancellationToken);

        var entity = await Repo.GetByIdWithNavigationsAsync(id, [a => a.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Avoir {id} introuvable.");

        // Signed difference between the stock effect the old lines had and the one the new
        // lines have, so editing quantities — or ticking / unticking the goods-return
        // checkbox — is corrected with a single movement per intrant.
        var deltas = AvoirStockEffect.Delta(
            entity.RetourMarchandise,
            entity.Lignes.Select(l => (l.IntrantId, l.Quantite)),
            dto.RetourMarchandise,
            dto.Lignes.Select(l => (l.IntrantId, l.Quantite)),
            signe: -1);

        entity.Lignes.Clear();
        Mapper.Map(dto, entity);
        entity.Motif = dto.Motif ?? string.Empty;
        NormalizeLines(entity.Lignes);

        foreach (var line in entity.Lignes)
        {
            line.Id = 0;
            line.AvoirFournisseurId = entity.Id;
        }

        await Repo.ExecuteInTransactionAsync(async ct =>
        {
            await _stockIntrantService.ApplyAvoirFournisseurAjustementAsync(
                id,
                deltas.Select(d => (d.Key, d.Value)),
                ct);

            await Repo.UpdateAsync(entity, ct);
        }, cancellationToken);
    }

    public async Task DeleteAvoirAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdWithNavigationsAsync(id, [a => a.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Avoir {id} introuvable.");

        await Repo.ExecuteInTransactionAsync(async ct =>
        {
            await _stockIntrantService.ReverseAvoirFournisseurAsync(
                id,
                entity.RetourMarchandise,
                entity.Lignes.Select(l => (l.IntrantId, l.Quantite)),
                ct);

            await DeleteAsync(id, ct);
        }, cancellationToken);
    }

    public async Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.Today.Year;
        var prefix = $"AF-{year}-";
        var existing = await Repo.FindAsync(a => a.Numero.StartsWith(prefix), cancellationToken);
        var next = existing
            .Select(a =>
            {
                var tail = a.Numero[prefix.Length..];
                return int.TryParse(tail, out var n) ? n : 0;
            })
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"{prefix}{next:D4}";
    }

    private static void NormalizeLines(IEnumerable<AvoirFournisseurLigne> lignes)
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
                new ValidationFailure(nameof(CreateAvoirFournisseurDto.FournisseurId),
                    "Le tiers sélectionné n'est pas un fournisseur.")]);
        }
    }
}
