namespace OliveMorocco.Business.DTOs.Operationnel;

public record SecteurSelectItemDto(int Id, string Nom);

public record IntrantSelectItemDto(int Id, string Nom, string Unite);

public record InterventionChargeListItemDto(
    int Id,
    string TypeChargeNom,
    string Libelle,
    DateTime Date,
    decimal MontantTtc);

public record InterventionDto(
    int Id,
    int SecteurId,
    string SecteurNom,
    DateTime Date,
    int? IntrantId,
    string? IntrantNom,
    string? IntrantUnite,
    decimal? QuantiteIntrant,
    decimal? QuantiteEau,
    string? Note,
    decimal TotalCharges,
    IReadOnlyList<InterventionChargeListItemDto> Charges);

public record CreateInterventionDto(
    int SecteurId,
    DateTime Date,
    int? IntrantId,
    decimal? QuantiteIntrant,
    decimal? QuantiteEau,
    string? Note);

public record UpdateInterventionDto(
    int SecteurId,
    DateTime Date,
    int? IntrantId,
    decimal? QuantiteIntrant,
    decimal? QuantiteEau,
    string? Note);

public record InterventionListItemDto(
    int Id,
    int SecteurId,
    string SecteurNom,
    DateTime Date,
    string? IntrantNom,
    string? IntrantUnite,
    decimal? QuantiteIntrant,
    decimal? QuantiteEau,
    decimal TotalCharges,
    string? Note);
