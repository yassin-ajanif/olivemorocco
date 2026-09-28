namespace OliveMorocco.Business.DTOs.Stockage;

public record StockHuileListItemDto(
    int VarieteId,
    string VarieteNom,
    string? Code,
    decimal StockHuile);

public record StockHuileDetailDto(
    int VarieteId,
    string VarieteNom,
    string? Code,
    string? RegionOrigine,
    decimal StockHuile);
