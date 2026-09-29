using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Vente;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Stockage;

/// <summary>
/// Owns the product stock ledger: <see cref="Produit"/> stock is the sum of its
/// <see cref="MouvementStock"/> rows (no persisted stock column), and every source —
/// bon de livraison, remplissage, manual adjustment — writes through this service.
/// </summary>
public sealed class StockService : IStockService
{
    /// <summary>Shipment to a client (out), or its correction / reversal on edit / delete.</summary>
    public const string OrigineBonLivraison = "BonLivraison";

    /// <summary>Goods reception from a supplier (in).</summary>
    public const string OrigineBonReception = "BR";

    /// <summary>Client credit note / return (in).</summary>
    public const string OrigineAvoirClient = "Avoir";

    /// <summary>Supplier credit note / return (out).</summary>
    public const string OrigineAvoirFournisseur = "AvoirFournisseur";

    /// <summary>Product import / manual adjustment.</summary>
    public const string OrigineImport = "Import";

    /// <summary>Bottles produced by a remplissage (in), or its correction / reversal on edit / delete.</summary>
    public const string OrigineRemplissage = "Remplissage";

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
                  || (p.CodeBarre != null && EF.Functions.ILike(p.CodeBarre, pattern))),
            query => query
                .OrderBy(p => p.Designation)
                .ThenBy(p => p.Reference),
            p => new StockEtatListItemDto(
                p.Id,
                p.Reference,
                p.Designation,
                p.Variete.Nom,
                p.Unite,
                ComputeStock(p),
                p.StockMinimum,
                p.Actif,
                ComputeStock(p) <= p.StockMinimum,
                ComputeStock(p) <= 0),
            page,
            pageSize,
            cancellationToken,
            [p => p.MouvementsStock]);

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

        var produit = await LoadProduitAsync(produitId, cancellationToken);

        var mouvement = BuildMouvement(
            produit,
            dto.Variation,
            OrigineImport,
            origineId: null,
            dto.Note,
            nameof(CreateAjustementStockDto.Variation));

        produit.UpdatedAt = DateTime.UtcNow;

        await _mouvements.AddAsync(mouvement, cancellationToken);
    }

    public async Task ApplyBonLivraisonSortieAsync(
        int bonLivraisonId,
        IEnumerable<(int ProduitId, decimal QuantiteLivree)> lignes,
        CancellationToken cancellationToken = default)
    {
        foreach (var (produitId, quantiteLivree) in lignes)
        {
            if (quantiteLivree <= 0)
                continue;

            await ApplyProduitMouvementAsync(
                produitId,
                -quantiteLivree,
                OrigineBonLivraison,
                bonLivraisonId,
                string.Empty,
                nameof(DTOs.Vente.CreateBonLivraisonClientDto.Lignes),
                cancellationToken);
        }
    }

    public async Task ApplyBonLivraisonAjustementAsync(
        int bonLivraisonId,
        IEnumerable<(int ProduitId, decimal Delta)> deltas,
        CancellationToken cancellationToken = default)
    {
        foreach (var (produitId, delta) in deltas)
        {
            if (delta == 0)
                continue;

            await ApplyProduitMouvementAsync(
                produitId,
                delta,
                OrigineBonLivraison,
                bonLivraisonId,
                "Modification du bon de livraison",
                nameof(DTOs.Vente.UpdateBonLivraisonClientDto.Lignes),
                cancellationToken);
        }
    }

    public async Task ReverseBonLivraisonAsync(
        int bonLivraisonId,
        IEnumerable<(int ProduitId, decimal QuantiteLivree)> lignes,
        CancellationToken cancellationToken = default)
    {
        foreach (var (produitId, quantiteLivree) in lignes)
        {
            if (quantiteLivree <= 0)
                continue;

            await ApplyProduitMouvementAsync(
                produitId,
                quantiteLivree,
                OrigineBonLivraison,
                bonLivraisonId,
                "Suppression du bon de livraison",
                string.Empty,
                cancellationToken);
        }
    }

    public async Task ApplyProduitMouvementAsync(
        int produitId,
        decimal variation,
        string origineType,
        int? origineId,
        string? note,
        string errorPropertyName,
        CancellationToken cancellationToken = default)
    {
        if (variation == 0)
            return;

        var produit = await LoadProduitAsync(produitId, cancellationToken);

        var mouvement = BuildMouvement(
            produit,
            variation,
            origineType,
            origineId,
            note,
            errorPropertyName);

        await _mouvements.AddAsync(mouvement, cancellationToken);
    }

    private async Task<Produit> LoadProduitAsync(int produitId, CancellationToken cancellationToken) =>
        await _produits.GetByIdWithNavigationsAsync(
            produitId, [p => p.MouvementsStock], cancellationToken)
        ?? throw new KeyNotFoundException($"Produit {produitId} introuvable.");

    private static StockProduitDetailDto ToDetailDto(Produit entity) =>
        new(
            entity.Id,
            entity.Reference,
            entity.Designation,
            entity.Variete.Nom,
            entity.Unite,
            ComputeStock(entity),
            entity.StockMinimum,
            ComputeStock(entity) <= entity.StockMinimum,
            ComputeStock(entity) <= 0);

    /// <summary>
    /// Product stock is derived from the movement ledger only — nothing is stored on the produit,
    /// so <see cref="Produit.MouvementsStock"/> must be loaded for the result to be meaningful.
    /// </summary>
    public static decimal ComputeStock(Produit produit) =>
        produit.MouvementsStock.Sum(m => m.Type == TypeMouvement.Entree ? m.Quantite : -m.Quantite);

    /// <summary>
    /// Builds a movement and rejects a negative resulting stock.
    /// The caller persists the returned movement.
    /// </summary>
    private static MouvementStock BuildMouvement(
        Produit produit,
        decimal variation,
        string origineType,
        int? origineId,
        string? note,
        string errorPropertyName)
    {
        var stockAvant = ComputeStock(produit);
        var nouveauStock = stockAvant + variation;

        if (nouveauStock < 0)
        {
            throw new ValidationException([
                new ValidationFailure(
                    errorPropertyName,
                    BuildStockInsuffisantMessage(produit, stockAvant, variation, origineType)),
            ]);
        }

        return new MouvementStock
        {
            ProduitId = produit.Id,
            Type = variation > 0 ? TypeMouvement.Entree : TypeMouvement.Sortie,
            Quantite = Math.Abs(variation),
            StockAvant = stockAvant,
            OrigineType = origineType,
            OrigineId = origineId,
            Note = note?.Trim() ?? string.Empty,
        };
    }

    /// <summary>The wording follows the source of the movement: manual adjustment, bottling, or a sales document.</summary>
    private static string BuildStockInsuffisantMessage(
        Produit produit,
        decimal stockAvant,
        decimal variation,
        string origineType) =>
        origineType switch
        {
            OrigineImport => "Le stock ne peut pas devenir négatif.",
            OrigineRemplissage =>
                $"Stock insuffisant pour « {produit.Reference} » : {stockAvant:N0} en stock, ces unités ont déjà été vendues ou ajustées.",
            _ =>
                $"Stock insuffisant pour « {produit.Reference} » : {stockAvant:N0} en stock, {Math.Abs(variation):N0} demandés.",
        };

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
            OrigineBonLivraison => origineId.HasValue ? $"BL #{origineId}" : "Bon de livraison",
            OrigineBonReception => origineId.HasValue ? $"BR #{origineId}" : "Bon de réception",
            OrigineAvoirClient => origineId.HasValue ? $"Avoir #{origineId}" : "Avoir client",
            OrigineAvoirFournisseur => origineId.HasValue ? $"Avoir fourn. #{origineId}" : "Avoir fournisseur",
            OrigineRemplissage => origineId.HasValue ? $"Remplissage #{origineId}" : "Remplissage",
            OrigineImport => "Ajustement manuel",
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
