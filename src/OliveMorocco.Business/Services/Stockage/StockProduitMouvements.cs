using FluentValidation;
using FluentValidation.Results;
using OliveMorocco.Domain.Entities.Vente;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Stockage;

/// <summary>
/// Builds product stock movements. Stock is computed from the movement table only.
/// The caller persists the returned movement.
/// </summary>
internal static class StockProduitMouvements
{
    public const string OrigineBonLivraison = "BonLivraison";
    public const string OrigineAjustement = "Ajustement";
    public const string OrigineRemplissage = "Remplissage";

    public static MouvementStock Apply(
        Produit produit,
        decimal variation,
        string origineType,
        int? origineId,
        string? note,
        string errorPropertyName,
        string negativeStockMessage)
    {
        var stockAvant = ComputeStock(produit);
        var nouveauStock = stockAvant + variation;

        if (nouveauStock < 0)
        {
            throw new ValidationException([
                new ValidationFailure(errorPropertyName, negativeStockMessage),
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

    public static decimal ComputeStock(Produit produit) =>
        produit.MouvementsStock.Sum(m => m.Type == TypeMouvement.Entree ? m.Quantite : -m.Quantite);
}
