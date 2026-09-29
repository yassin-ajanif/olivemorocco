using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.DTOs.Stockage;

public record IntrantStockListItemDto(
    int Id,
    string Nom,
    string Unite,
    decimal StockActuel,
    decimal? PrixAchatHT);

public record IntrantStockDetailDto(
    int Id,
    string Nom,
    string Unite,
    decimal StockActuel,
    decimal? PrixAchatHT);

public record MouvementIntrantListItemDto(
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

public record CreateAjustementIntrantDto(
    decimal Variation,
    string? Note);
