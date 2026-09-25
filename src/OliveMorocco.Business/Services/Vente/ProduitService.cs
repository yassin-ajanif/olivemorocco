using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Business.Services.Vente;

public sealed class ProduitService : IProduitService
{
    private readonly IRepository<Produit> _produits;
    private readonly IRepository<Variete> _varietes;
    private readonly IRepository<DevisClientLigne> _devisLignes;
    private readonly IRepository<BonCommandeClientLigne> _bonCommandeLignes;
    private readonly IRepository<BonLivraisonClientLigne> _bonLivraisonLignes;
    private readonly IRepository<FactureClientLigne> _factureLignes;
    private readonly IRepository<AvoirClientLigne> _avoirLignes;
    private readonly IRepository<MouvementStock> _mouvementsStock;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProduitDto>? _createValidator;
    private readonly IValidator<UpdateProduitDto>? _updateValidator;

    public ProduitService(
        IRepository<Produit> produits,
        IRepository<Variete> varietes,
        IRepository<DevisClientLigne> devisLignes,
        IRepository<BonCommandeClientLigne> bonCommandeLignes,
        IRepository<BonLivraisonClientLigne> bonLivraisonLignes,
        IRepository<FactureClientLigne> factureLignes,
        IRepository<AvoirClientLigne> avoirLignes,
        IRepository<MouvementStock> mouvementsStock,
        IMapper mapper,
        IEnumerable<IValidator<CreateProduitDto>> createValidators,
        IEnumerable<IValidator<UpdateProduitDto>> updateValidators)
    {
        _produits = produits;
        _varietes = varietes;
        _devisLignes = devisLignes;
        _bonCommandeLignes = bonCommandeLignes;
        _bonLivraisonLignes = bonLivraisonLignes;
        _factureLignes = factureLignes;
        _avoirLignes = avoirLignes;
        _mouvementsStock = mouvementsStock;
        _mapper = mapper;
        _createValidator = createValidators.FirstOrDefault();
        _updateValidator = updateValidators.FirstOrDefault();
    }

    public async Task<PagedResult<ProduitListItemDto>> GetProduitsAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var (items, totalCount) = await _produits.QueryPagedAsync(
            p => pattern == null
                 || EF.Functions.ILike(p.Reference, pattern)
                 || EF.Functions.ILike(p.Designation, pattern)
                 || (p.CodeBarre != null && EF.Functions.ILike(p.CodeBarre, pattern)),
            query => query.OrderBy(p => p.Designation).ThenBy(p => p.Reference),
            p => new ProduitListItemDto(
                p.Id,
                p.Reference,
                p.Designation,
                p.Variete != null ? p.Variete.Nom : null,
                p.Unite,
                p.PrixVenteHT,
                p.StockActuel,
                p.StockMinimum,
                p.Actif,
                p.StockActuel <= p.StockMinimum),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<ProduitListItemDto>(items, totalCount);
    }

    public async Task<ProduitDto?> GetProduitByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _produits.GetByIdWithNavigationsAsync(
            id,
            [p => p.Variete!],
            cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<VarieteSelectItemDto>> GetVarietesForSelectAsync(
        CancellationToken cancellationToken = default)
    {
        var (items, _) = await _varietes.QueryPagedAsync(
            null,
            query => query.OrderBy(v => v.Nom),
            v => new VarieteSelectItemDto(v.Id, v.Nom),
            page: 1,
            pageSize: 500,
            cancellationToken);

        return items;
    }

    public async Task<ProduitDto> CreateProduitAsync(
        CreateProduitDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);
        await EnsureReferenceUniqueAsync(dto.Reference, excludeId: null, cancellationToken);
        await EnsureVarieteExistsAsync(dto.VarieteId, cancellationToken);

        var entity = _mapper.Map<Produit>(dto);
        entity.StockActuel = dto.StockInitial;

        await _produits.AddAsync(entity, cancellationToken);
        return (await GetProduitByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateProduitAsync(
        int id,
        UpdateProduitDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, dto, cancellationToken);
        await EnsureReferenceUniqueAsync(dto.Reference, excludeId: id, cancellationToken);
        await EnsureVarieteExistsAsync(dto.VarieteId, cancellationToken);

        var entity = await _produits.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Produit {id} introuvable.");

        _mapper.Map(dto, entity);
        await _produits.UpdateAsync(entity, cancellationToken);
    }

    public async Task DeleteProduitAsync(int id, CancellationToken cancellationToken = default)
    {
        _ = await _produits.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Produit {id} introuvable.");

        await EnsureCanDeleteAsync(id, cancellationToken);
        await _produits.DeleteAsync(id, cancellationToken);
    }

    public async Task<ProduitDto> ToggleActifAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _produits.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Produit {id} introuvable.");

        entity.Actif = !entity.Actif;
        await _produits.UpdateAsync(entity, cancellationToken);
        return (await GetProduitByIdAsync(id, cancellationToken))!;
    }

    private static ProduitDto ToDto(Produit entity) =>
        new(
            entity.Id,
            entity.Reference,
            entity.Designation,
            entity.VarieteId,
            entity.Variete?.Nom,
            entity.Unite,
            entity.CodeBarre,
            entity.PrixAchatHT,
            entity.PrixVenteHT,
            entity.TauxTVA,
            entity.StockActuel,
            entity.StockMinimum,
            entity.Actif);

    private async Task EnsureReferenceUniqueAsync(
        string reference,
        int? excludeId,
        CancellationToken cancellationToken)
    {
        var normalized = reference.Trim();
        if (await _produits.AnyAsync(
                p => p.Reference == normalized && (excludeId == null || p.Id != excludeId),
                cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateProduitDto.Reference),
                    "Cette référence est déjà utilisée par un autre produit."),
            ]);
        }
    }

    private async Task EnsureVarieteExistsAsync(int? varieteId, CancellationToken cancellationToken)
    {
        if (varieteId is null)
            return;

        if (!await _varietes.AnyAsync(v => v.Id == varieteId.Value, cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateProduitDto.VarieteId),
                    "La variété sélectionnée est introuvable."),
            ]);
        }
    }

    private async Task EnsureCanDeleteAsync(int id, CancellationToken cancellationToken)
    {
        var linked = new List<string>();

        if (await _devisLignes.AnyAsync(l => l.ProduitId == id, cancellationToken))
            linked.Add("Devis client");
        if (await _bonCommandeLignes.AnyAsync(l => l.ProduitId == id, cancellationToken))
            linked.Add("Bons de commande client");
        if (await _bonLivraisonLignes.AnyAsync(l => l.ProduitId == id, cancellationToken))
            linked.Add("Bons de livraison client");
        if (await _factureLignes.AnyAsync(l => l.ProduitId == id, cancellationToken))
            linked.Add("Factures client");
        if (await _avoirLignes.AnyAsync(l => l.ProduitId == id, cancellationToken))
            linked.Add("Avoirs client");
        if (await _mouvementsStock.AnyAsync(m => m.ProduitId == id, cancellationToken))
            linked.Add("Mouvements de stock");

        if (linked.Count == 0)
            return;

        var message = "Impossible de supprimer ce produit.\n\nDocuments liés :\n"
                      + string.Join('\n', linked.Select(l => $"• {l}"))
                      + "\n\nDésactivez-le à la place.";

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
