using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Vente;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Stockage;

public sealed class StockService : IStockService
{
    private readonly IRepository<Produit> _produits;
    private readonly IRepository<MouvementStock> _mouvements;
    private readonly IValidator<CreateAjustementStockDto>? _ajustementValidator;

    public StockService(
        IRepository<Produit> produits,
        IRepository<MouvementStock> mouvements,
        IEnumerable<IValidator<CreateAjustementStockDto>> ajustementValidators)
    {
        _produits = produits;
        _mouvements = mouvements;
        _ajustementValidator = ajustementValidators.FirstOrDefault();
    }

    public async Task<PagedResult<StockEtatListItemDto>> GetStockEtatAsync(
        string? search = null,
        bool stockBasOnly = false,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var (items, totalCount) = await _produits.QueryPagedAsync(
            p => (pattern == null
                  || EF.Functions.ILike(p.Reference, pattern)
                  || EF.Functions.ILike(p.Designation, pattern)
                  || (p.CodeBarre != null && EF.Functions.ILike(p.CodeBarre, pattern)))
                 && (!stockBasOnly || p.StockActuel <= p.StockMinimum),
            query => query
                .OrderByDescending(p => p.StockActuel <= p.StockMinimum)
                .ThenBy(p => p.Designation)
                .ThenBy(p => p.Reference),
            p => new StockEtatListItemDto(
                p.Id,
                p.Reference,
                p.Designation,
                p.Variete != null ? p.Variete.Nom : null,
                p.Unite,
                p.StockActuel,
                p.StockMinimum,
                p.Actif,
                p.StockActuel <= p.StockMinimum,
                p.StockActuel <= 0),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<StockEtatListItemDto>(items, totalCount);
    }

    public async Task<StockProduitDetailDto?> GetProduitStockDetailAsync(
        int produitId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _produits.GetByIdWithNavigationsAsync(
            produitId,
            [p => p.Variete!],
            cancellationToken);

        return entity is null ? null : ToDetailDto(entity);
    }

    public async Task<PagedResult<MouvementStockListItemDto>> GetMouvementsAsync(
        int produitId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        if (!await _produits.AnyAsync(p => p.Id == produitId, cancellationToken))
            throw new KeyNotFoundException($"Produit {produitId} introuvable.");

        var (items, totalCount) = await _mouvements.QueryPagedAsync(
            m => m.ProduitId == produitId,
            query => query.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id),
            m => new MouvementStockListItemDto(
                m.Id,
                m.CreatedAt,
                m.Type,
                m.Quantite,
                m.StockAvant,
                ComputeStockApres(m.Type, m.StockAvant, m.Quantite),
                m.OrigineType,
                m.OrigineId,
                FormatOrigineLabel(m.OrigineType, m.OrigineId),
                m.Note),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<MouvementStockListItemDto>(items, totalCount);
    }

    public async Task CreateAjustementAsync(
        int produitId,
        CreateAjustementStockDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_ajustementValidator, dto, cancellationToken);

        var produit = await _produits.GetByIdAsync(produitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Produit {produitId} introuvable.");

        var stockAvant = produit.StockActuel;
        var nouveauStock = stockAvant + dto.Variation;

        if (nouveauStock < 0)
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateAjustementStockDto.Variation),
                    "Le stock ne peut pas devenir négatif."),
            ]);
        }

        var type = dto.Variation > 0 ? TypeMouvement.Entree : TypeMouvement.Sortie;
        var now = DateTime.UtcNow;

        var mouvement = new MouvementStock
        {
            ProduitId = produitId,
            Type = type,
            Quantite = Math.Abs(dto.Variation),
            StockAvant = stockAvant,
            OrigineType = "Import",
            OrigineId = null,
            Note = dto.Note?.Trim() ?? string.Empty,
            CreatedAt = now,
            UpdatedAt = now,
        };

        produit.StockActuel = nouveauStock;
        produit.UpdatedAt = now;

        await _mouvements.AddAsync(mouvement, cancellationToken);
    }

    private static StockProduitDetailDto ToDetailDto(Produit entity) =>
        new(
            entity.Id,
            entity.Reference,
            entity.Designation,
            entity.Variete?.Nom,
            entity.Unite,
            entity.StockActuel,
            entity.StockMinimum,
            entity.StockActuel <= entity.StockMinimum,
            entity.StockActuel <= 0);

    private static decimal ComputeStockApres(TypeMouvement type, decimal stockAvant, decimal quantite) =>
        type switch
        {
            TypeMouvement.Entree => stockAvant + quantite,
            TypeMouvement.Sortie => stockAvant - quantite,
            _ => stockAvant + quantite,
        };

    private static string FormatOrigineLabel(string origineType, int? origineId) =>
        origineType switch
        {
            "BL" => origineId.HasValue ? $"BL #{origineId}" : "Bon de livraison",
            "BR" => origineId.HasValue ? $"BR #{origineId}" : "Bon de réception",
            "Avoir" => origineId.HasValue ? $"Avoir #{origineId}" : "Avoir client",
            "AvoirFournisseur" => origineId.HasValue ? $"Avoir fourn. #{origineId}" : "Avoir fournisseur",
            "Import" => "Ajustement manuel",
            _ => origineId.HasValue ? $"{origineType} #{origineId}" : origineType,
        };

    private static async Task ValidateAsync<T>(
        IValidator<T>? validator,
        T dto,
        CancellationToken cancellationToken)
    {
        if (validator is null)
            return;

        var result = await validator.ValidateAsync(dto, cancellationToken);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}
