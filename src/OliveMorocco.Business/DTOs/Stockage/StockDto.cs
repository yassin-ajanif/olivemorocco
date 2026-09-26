using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.DTOs.Stockage;

public record StockEtatListItemDto(
    int Id,
    string Reference,
    string Designation,
    string VarieteNom,
    string Unite,
    decimal StockActuel,
    decimal StockMinimum,
    bool Actif,
    bool StockBas,
    bool Rupture);

public record StockProduitDetailDto(
    int Id,
    string Reference,
    string Designation,
    string VarieteNom,
    string Unite,
    decimal StockActuel,
    decimal StockMinimum,
    bool StockBas,
    bool Rupture);

public record MouvementStockListItemDto(
    int Id,
    DateTime Date,
    TypeMouvement Type,
    decimal Quantite,
    decimal StockAvant,
    decimal StockApres,
    string OrigineType,
    int? OrigineId,
    string OrigineLabel,
    string Note);

public record CreateAjustementStockDto(
    decimal Variation,
    string? Note);
