namespace OliveMorocco.Business.DTOs.Operationnel;

public record FournisseurSelectItemDto(int Id, string Nom);

public record FactureFournisseurSelectItemDto(int Id, string Numero, DateTime Date);

public record PressageDto(
    int Id,
    int FournisseurId,
    string FournisseurNom,
    int VarieteId,
    string VarieteNom,
    DateTime Date,
    decimal QuantiteOlives,
    decimal Rendement,
    decimal? QuantiteHuile,
    int? FactureFournisseurId,
    string? FactureNumero);

public record CreatePressageDto(
    int FournisseurId,
    int VarieteId,
    DateTime Date,
    decimal QuantiteOlives,
    decimal Rendement,
    decimal? QuantiteHuile,
    int? FactureFournisseurId);

public record UpdatePressageDto(
    int FournisseurId,
    int VarieteId,
    DateTime Date,
    decimal QuantiteOlives,
    decimal Rendement,
    decimal? QuantiteHuile,
    int? FactureFournisseurId);

public record PressageListItemDto(
    int Id,
    DateTime Date,
    string FournisseurNom,
    string VarieteNom,
    decimal QuantiteOlives,
    decimal Rendement,
    decimal? QuantiteHuile,
    string? FactureNumero);
