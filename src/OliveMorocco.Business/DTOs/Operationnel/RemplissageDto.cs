namespace OliveMorocco.Business.DTOs.Operationnel;

public record RemplissageVarieteSelectItemDto(int Id, string Nom, decimal StockHuile);

public record RemplissageProduitSelectItemDto(
    int Id,
    int VarieteId,
    string Reference,
    string Designation,
    string Unite,
    decimal ContenanceLitres,
    decimal StockActuel);

public record RemplissageLigneDto(
    int Id,
    int ProduitId,
    string ProduitReference,
    string ProduitDesignation,
    decimal Quantite,
    decimal ContenanceLitres,
    decimal Litres);

public record RemplissageDto(
    int Id,
    string Numero,
    int VarieteId,
    string VarieteNom,
    DateTime Date,
    decimal QuantiteHuile,
    decimal Perte,
    string? Note,
    IReadOnlyList<RemplissageLigneDto> Lignes);

public record CreateRemplissageLigneDto(int ProduitId, decimal Quantite);

public record CreateRemplissageDto(
    int VarieteId,
    DateTime Date,
    decimal Perte,
    string? Note,
    IReadOnlyList<CreateRemplissageLigneDto> Lignes);

public record UpdateRemplissageDto(
    int VarieteId,
    DateTime Date,
    decimal Perte,
    string? Note,
    IReadOnlyList<CreateRemplissageLigneDto> Lignes);

public record RemplissageListItemDto(
    int Id,
    string Numero,
    DateTime Date,
    string VarieteNom,
    string ProduitsResume,
    decimal QuantiteHuile,
    decimal Perte);
