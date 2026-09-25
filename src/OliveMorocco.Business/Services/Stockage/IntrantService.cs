using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Business.Services.Stockage;

public sealed class IntrantService : IIntrantService
{
    private readonly IRepository<Intrant> _intrants;
    private readonly IRepository<InterventionLigne> _interventionLignes;
    private readonly IRepository<BonCommandeFournisseurLigne> _bonCommandeLignes;
    private readonly IRepository<BonReceptionLigne> _bonReceptionLignes;
    private readonly IRepository<FactureFournisseurLigne> _factureLignes;
    private readonly IRepository<AvoirFournisseurLigne> _avoirLignes;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateIntrantDto>? _createValidator;
    private readonly IValidator<UpdateIntrantDto>? _updateValidator;

    public IntrantService(
        IRepository<Intrant> intrants,
        IRepository<InterventionLigne> interventionLignes,
        IRepository<BonCommandeFournisseurLigne> bonCommandeLignes,
        IRepository<BonReceptionLigne> bonReceptionLignes,
        IRepository<FactureFournisseurLigne> factureLignes,
        IRepository<AvoirFournisseurLigne> avoirLignes,
        IMapper mapper,
        IEnumerable<IValidator<CreateIntrantDto>> createValidators,
        IEnumerable<IValidator<UpdateIntrantDto>> updateValidators)
    {
        _intrants = intrants;
        _interventionLignes = interventionLignes;
        _bonCommandeLignes = bonCommandeLignes;
        _bonReceptionLignes = bonReceptionLignes;
        _factureLignes = factureLignes;
        _avoirLignes = avoirLignes;
        _mapper = mapper;
        _createValidator = createValidators.FirstOrDefault();
        _updateValidator = updateValidators.FirstOrDefault();
    }

    public async Task<PagedResult<IntrantListItemDto>> GetIntrantsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var (items, totalCount) = await _intrants.QueryPagedAsync(
            i => pattern == null || EF.Functions.ILike(i.Nom, pattern),
            query => query.OrderBy(i => i.Nom),
            i => new IntrantListItemDto(i.Id, i.Nom, i.Unite),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<IntrantListItemDto>(items, totalCount);
    }

    public async Task<IntrantDto?> GetIntrantByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _intrants.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<IntrantDto> CreateIntrantAsync(
        CreateIntrantDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);
        await EnsureNomUniqueAsync(dto.Nom, excludeId: null, cancellationToken);

        var entity = _mapper.Map<Intrant>(dto);
        await _intrants.AddAsync(entity, cancellationToken);

        return (await GetIntrantByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateIntrantAsync(
        int id,
        UpdateIntrantDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, dto, cancellationToken);
        await EnsureNomUniqueAsync(dto.Nom, excludeId: id, cancellationToken);

        var entity = await _intrants.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Intrant {id} introuvable.");

        _mapper.Map(dto, entity);
        await _intrants.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteIntrantAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await _intrants.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Intrant {id} introuvable.");

        await EnsureCanDeleteAsync(id, cancellationToken);
        await _intrants.DeleteAsync(id, cancellationToken);
    }

    private static IntrantDto ToDto(Intrant entity) =>
        new(entity.Id, entity.Nom, entity.Unite);

    private async Task EnsureNomUniqueAsync(
        string nom,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        var normalized = nom.Trim();
        if (await _intrants.AnyAsync(
                i => i.Nom == normalized && (excludeId == null || i.Id != excludeId),
                cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateIntrantDto.Nom),
                    "Un intrant avec ce nom existe déjà."),
            ]);
        }
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await _interventionLignes.AnyAsync(l => l.IntrantId == id, cancellationToken))
            linked.Add("Interventions");
        if (await _bonCommandeLignes.AnyAsync(l => l.IntrantId == id, cancellationToken))
            linked.Add("Bons de commande fournisseur");
        if (await _bonReceptionLignes.AnyAsync(l => l.IntrantId == id, cancellationToken))
            linked.Add("Bons de réception");
        if (await _factureLignes.AnyAsync(l => l.IntrantId == id, cancellationToken))
            linked.Add("Factures fournisseur");
        if (await _avoirLignes.AnyAsync(l => l.IntrantId == id, cancellationToken))
            linked.Add("Avoirs fournisseur");

        if (linked.Count == 0)
            return;

        var message = "Impossible de supprimer cet intrant.\n\nDocuments liés :\n"
                      + string.Join('\n', linked.Select(l => $"• {l}"));

        throw new ValidationException([
            new ValidationFailure(string.Empty, message),
        ]);
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
