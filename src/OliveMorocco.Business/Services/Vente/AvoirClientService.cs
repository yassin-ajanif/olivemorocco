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

public sealed class AvoirClientService
    : GenericService<AvoirClient, AvoirClientDto, CreateAvoirClientDto, UpdateAvoirClientDto>,
      IAvoirClientService
{
    private readonly IRepository<AvoirClientLigne> _lignes;
    private readonly IRepository<Tiers> _tiers;
    private readonly IRepository<FactureClient> _factures;

    public AvoirClientService(
        IRepository<AvoirClient> avoirs,
        IRepository<AvoirClientLigne> lignes,
        IRepository<Tiers> tiers,
        IRepository<FactureClient> factures,
        IMapper mapper,
        IEnumerable<IValidator<CreateAvoirClientDto>> createValidators,
        IEnumerable<IValidator<UpdateAvoirClientDto>> updateValidators)
        : base(avoirs, mapper, createValidators, updateValidators)
    {
        _lignes = lignes;
        _tiers = tiers;
        _factures = factures;
    }

    public async Task<PagedResult<AvoirClientListItemDto>> GetAvoirsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var predicate = (System.Linq.Expressions.Expression<Func<AvoirClient, bool>>?)(a =>
            pattern == null
            || EF.Functions.ILike(a.Numero, pattern)
            || EF.Functions.ILike(a.Client.Nom, pattern)
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
            return new PagedResult<AvoirClientListItemDto>([], totalCount);

        var headers = await Repo.FindWithIncludesAsync(
            a => ids.Contains(a.Id),
            [a => a.Client, a => a.Lignes],
            cancellationToken);

        var byId = headers.ToDictionary(a => a.Id);
        var listItems = items
            .Where(a => byId.ContainsKey(a.Id))
            .Select(a =>
            {
                var full = byId[a.Id];
                var (_, _, ttc) = IAvoirClientService.ComputeTotals(
                    full.Lignes.Select(l => new CreateAvoirClientLigneDto(
                        l.ProduitId,
                        l.Designation,
                        l.Quantite,
                        l.PrixUnitaireHT,
                        l.Remise,
                        l.TauxTVA,
                        l.Conditionnement)));

                return new AvoirClientListItemDto(
                    full.Id,
                    full.Numero,
                    full.ClientId,
                    full.Client.Nom,
                    full.Date,
                    ttc,
                    full.Motif,
                    full.RetourMarchandise);
            })
            .ToList();

        return new PagedResult<AvoirClientListItemDto>(listItems, totalCount);
    }

    public async Task<AvoirClientDto?> GetAvoirByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdWithNavigationsAsync(
            id,
            [a => a.Client, a => a.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var ligneDtos = await _lignes.FindWithIncludesAsync(
            l => l.AvoirClientId == id,
            [l => l.Produit!],
            cancellationToken);

        var (_, _, ttc) = IAvoirClientService.ComputeTotals(
            ligneDtos.Select(l => new CreateAvoirClientLigneDto(
                l.ProduitId,
                l.Designation,
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA,
                l.Conditionnement)));

        return new AvoirClientDto(
            entity.Id,
            entity.Numero,
            entity.ClientId,
            entity.FactureId,
            entity.Date,
            entity.Motif,
            entity.RetourMarchandise,
            ttc,
            ligneDtos.Select(l => new AvoirClientLigneDto(
                l.Id,
                l.AvoirClientId,
                l.ProduitId,
                l.Produit?.Reference ?? string.Empty,
                l.Designation,
                l.Conditionnement,
                l.Quantite,
                l.PrixUnitaireHT,
                l.Remise,
                l.TauxTVA)).ToList());
    }

    public async Task<AvoirClientDto> CreateAvoirAsync(
        CreateAvoirClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);
        await EnsureClientExistsAsync(dto.ClientId, cancellationToken);
        await EnsureFactureMatchesClientAsync(dto.FactureId, dto.ClientId, cancellationToken);

        var numero = string.IsNullOrWhiteSpace(dto.Numero)
            ? await GenerateNumeroAsync(cancellationToken)
            : dto.Numero.Trim();

        var entity = Mapper.Map<AvoirClient>(dto with { Numero = numero });
        entity.Motif = dto.Motif ?? string.Empty;
        NormalizeLines(entity.Lignes);

        await Repo.AddAsync(entity, cancellationToken);
        return (await GetAvoirByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateAvoirAsync(
        int id,
        UpdateAvoirClientDto dto,
        CancellationToken cancellationToken = default)
    {
        await EnsureClientExistsAsync(dto.ClientId, cancellationToken);
        await EnsureFactureMatchesClientAsync(dto.FactureId, dto.ClientId, cancellationToken);
        await ValidateAsync(UpdateValidator, dto, cancellationToken);

        var entity = await Repo.GetByIdWithNavigationsAsync(id, [a => a.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Avoir {id} introuvable.");

        entity.Lignes.Clear();
        Mapper.Map(dto, entity);
        entity.Motif = dto.Motif ?? string.Empty;
        NormalizeLines(entity.Lignes);

        foreach (var line in entity.Lignes)
        {
            line.Id = 0;
            line.AvoirClientId = entity.Id;
        }

        await Repo.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteAvoirAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await GetAvoirByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Avoir {id} introuvable.");

        await DeleteAsync(id, cancellationToken);
    }

    public async Task<string> GenerateNumeroAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.Today.Year;
        var prefix = $"AV-{year}-";
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

    private static void NormalizeLines(IEnumerable<AvoirClientLigne> lignes)
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
                new ValidationFailure(nameof(CreateAvoirClientDto.ClientId),
                    "Le tiers sélectionné n'est pas un client.")]);
        }
    }

    private async Task EnsureFactureMatchesClientAsync(
        int? factureId,
        int clientId,
        CancellationToken cancellationToken)
    {
        if (factureId is not int id)
            return;

        var facture = await _factures.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Facture {id} introuvable.");

        if (facture.ClientId != clientId)
        {
            throw new ValidationException([
                new ValidationFailure(nameof(CreateAvoirClientDto.FactureId),
                    "La facture liée n'appartient pas au même client.")]);
        }
    }
}
