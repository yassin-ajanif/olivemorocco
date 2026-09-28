namespace OliveMorocco.Business.DTOs.Operationnel;

public record FournisseurSelectItemDto(int Id, string Nom);

public record FactureFournisseurSelectItemDto(int Id, string Numero, DateTime Date);

public record PressageDto(
    int Id,
    string Numero,
    int FournisseurId,
    string FournisseurNom,
    int VarieteId,
    string VarieteNom,
    DateTime Date,
    decimal QuantiteOlives,
    decimal Rendement,
    decimal? QuantiteHuile);

public record CreatePressageDto(
    int FournisseurId,
    int VarieteId,
    DateTime Date,
    decimal QuantiteOlives,
    decimal Rendement,
    decimal? QuantiteHuile,
    int TypeChargeId,
    string Libelle,
    DateTime ChargeDate,
    decimal MontantTtc,
    string? Note);

public record UpdatePressageDto(
    int FournisseurId,
    int VarieteId,
    DateTime Date,
    decimal QuantiteOlives,
    decimal Rendement,
    decimal? QuantiteHuile);

public record PressageListItemDto(
    int Id,
    string Numero,
    DateTime Date,
    string FournisseurNom,
    string VarieteNom,
    decimal QuantiteOlives,
    decimal Rendement,
    decimal? QuantiteHuile);
