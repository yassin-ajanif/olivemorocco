using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Services.Stockage;

public sealed class VarieteService : IVarieteService
{
    private readonly IRepository<Variete> _varietes;
    private readonly IRepository<Produit> _produits;
    private readonly IRepository<SecteurVariete> _secteurVarietes;
    private readonly IRepository<Recolte> _recoltes;
    private readonly IRepository<Pressage> _pressages;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateVarieteDto>? _createValidator;
    private readonly IValidator<UpdateVarieteDto>? _updateValidator;

    public VarieteService(
        IRepository<Variete> varietes,
        IRepository<Produit> produits,
        IRepository<SecteurVariete> secteurVarietes,
        IRepository<Recolte> recoltes,
        IRepository<Pressage> pressages,
        IMapper mapper,
        IEnumerable<IValidator<CreateVarieteDto>> createValidators,
        IEnumerable<IValidator<UpdateVarieteDto>> updateValidators)
    {
        _varietes = varietes;
        _produits = produits;
        _secteurVarietes = secteurVarietes;
        _recoltes = recoltes;
        _pressages = pressages;
        _mapper = mapper;
        _createValidator = createValidators.FirstOrDefault();
        _updateValidator = updateValidators.FirstOrDefault();
    }

    public async Task<PagedResult<VarieteListItemDto>> GetVarietesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var (items, totalCount) = await _varietes.QueryPagedAsync(
            v => pattern == null
                 || EF.Functions.ILike(v.Nom, pattern)
                 || (v.Code != null && EF.Functions.ILike(v.Code, pattern))
                 || (v.RegionOrigine != null && EF.Functions.ILike(v.RegionOrigine, pattern)),
            query => query.OrderBy(v => v.Nom),
            v => new VarieteListItemDto(v.Id, v.Nom, v.Code, v.RegionOrigine),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<VarieteListItemDto>(items, totalCount);
    }

    public async Task<VarieteDto?> GetVarieteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _varietes.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<VarieteCreatedDto> CreateVarieteAsync(
        CreateVarieteDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);
        await EnsureNomUniqueAsync(dto.Nom, excludeId: null, cancellationToken);
        await EnsureCodeUniqueAsync(dto.Code, excludeId: null, cancellationToken);

        var entity = _mapper.Map<Variete>(dto);
        await _varietes.AddAsync(entity, cancellationToken);

        return new VarieteCreatedDto(entity.Id, entity.Nom);
    }

    public async Task<VarieteDto> UpdateVarieteAsync(
        int id,
        UpdateVarieteDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, dto, cancellationToken);
        await EnsureNomUniqueAsync(dto.Nom, excludeId: id, cancellationToken);
        await EnsureCodeUniqueAsync(dto.Code, excludeId: id, cancellationToken);

        var entity = await _varietes.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Variété {id} introuvable.");

        _mapper.Map(dto, entity);
        await _varietes.UpdateAsync(entity, cancellationToken);

        return ToDto(entity);
    }

    public async Task DeleteVarieteAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await _varietes.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Variété {id} introuvable.");

        await EnsureCanDeleteAsync(id, cancellationToken);
        await _varietes.DeleteAsync(id, cancellationToken);
    }

    private static VarieteDto ToDto(Variete entity) =>
        new(entity.Id, entity.Nom, entity.Code, entity.RegionOrigine);

    private async Task EnsureNomUniqueAsync(
        string nom,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        var normalized = nom.Trim();
        if (await _varietes.AnyAsync(
                v => v.Nom == normalized && (excludeId == null || v.Id != excludeId),
                cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateVarieteDto.Nom),
                    "Une variété avec ce nom existe déjà."),
            ]);
        }
    }

    private async Task EnsureCodeUniqueAsync(
        string? code,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;

        var normalized = code.Trim();
        if (await _varietes.AnyAsync(
                v => v.Code == normalized && (excludeId == null || v.Id != excludeId),
                cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateVarieteDto.Code),
                    "Une variété avec ce code existe déjà."),
            ]);
        }
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await _produits.AnyAsync(p => p.VarieteId == id, cancellationToken))
            linked.Add("Produits");
        if (await _secteurVarietes.AnyAsync(sv => sv.VarieteId == id, cancellationToken))
            linked.Add("Secteurs");
        if (await _recoltes.AnyAsync(r => r.VarieteId == id, cancellationToken))
            linked.Add("Récoltes");
        if (await _pressages.AnyAsync(p => p.VarieteId == id, cancellationToken))
            linked.Add("Pressages");

        if (linked.Count == 0)
            return;

        var message = "Impossible de supprimer cette variété.\n\nÉléments liés :\n"
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
