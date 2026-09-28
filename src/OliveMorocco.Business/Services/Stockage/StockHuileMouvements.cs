using FluentValidation;
using FluentValidation.Results;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Stockage;

/// <summary>
/// Builds bulk oil movements and keeps <see cref="Variete.StockHuile"/> in sync.
/// The caller persists the returned movement (which also saves the tracked variété).
/// </summary>
internal static class StockHuileMouvements
{
    public const string OriginePressage = "Pressage";
    public const string OrigineImport = "Import";
    public const string OrigineRemplissage = "Remplissage";

    public static MouvementStockVariete Apply(
        Variete variete,
        decimal variation,
        string origineType,
        int? origineId,
        string? note,
        string errorPropertyName,
        string negativeStockMessage)
    {
        var stockAvant = variete.StockHuile;
        var nouveauStock = stockAvant + variation;

        if (nouveauStock < 0)
        {
            throw new ValidationException([
                new ValidationFailure(errorPropertyName, negativeStockMessage),
            ]);
        }

        variete.StockHuile = nouveauStock;

        return new MouvementStockVariete
        {
            VarieteId = variete.Id,
            Type = variation > 0 ? TypeMouvement.Entree : TypeMouvement.Sortie,
            Quantite = Math.Abs(variation),
            StockAvant = stockAvant,
            OrigineType = origineType,
            OrigineId = origineId,
            Note = note?.Trim() ?? string.Empty,
        };
    }

    public static string FormatOrigineLabel(string origineType, int? origineId) =>
        origineType switch
        {
            OriginePressage => origineId.HasValue ? $"Pressage #{origineId}" : "Pressage",
            OrigineRemplissage => origineId.HasValue ? $"Remplissage #{origineId}" : "Remplissage",
            OrigineImport => "Ajustement manuel",
            _ => origineId.HasValue ? $"{origineType} #{origineId}" : origineType,
        };
}
