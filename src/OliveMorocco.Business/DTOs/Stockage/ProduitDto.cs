namespace OliveMorocco.Business.DTOs.Stockage;

public record VarieteSelectItemDto(int Id, string Nom);

public record ProduitDto(
    int Id,
    string Reference,
    string Designation,
    int? VarieteId,
    string? VarieteNom,
    string Unite,
    string? CodeBarre,
    decimal PrixAchatHT,
    decimal PrixVenteHT,
    decimal TauxTVA,
    decimal StockActuel,
    decimal StockMinimum,
    bool Actif);

public record CreateProduitDto(
    string Reference,
    string Designation,
    int? VarieteId,
    string Unite,
    string? CodeBarre,
    decimal PrixAchatHT,
    decimal PrixVenteHT,
    decimal TauxTVA,
    decimal StockInitial,
    decimal StockMinimum,
    bool Actif);

public record UpdateProduitDto(
    string Reference,
    string Designation,
    int? VarieteId,
    string Unite,
    string? CodeBarre,
    decimal PrixAchatHT,
    decimal PrixVenteHT,
    decimal TauxTVA,
    decimal StockMinimum,
    bool Actif);

public record ProduitListItemDto(
    int Id,
    string Reference,
    string Designation,
    string? VarieteNom,
    string Unite,
    decimal PrixVenteHT,
    decimal StockActuel,
    decimal StockMinimum,
    bool Actif,
    bool StockBas);
