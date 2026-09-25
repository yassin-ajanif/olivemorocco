namespace OliveMorocco.Business.DTOs.Operationnel;

public record SecteurSelectItemDto(int Id, string Nom);

public record IntrantSelectItemDto(int Id, string Nom, string Unite);

public record InterventionLigneDto(
    int Id,
    int InterventionId,
    int IntrantId,
    string IntrantNom,
    string IntrantUnite,
    decimal Quantite);

public record CreateInterventionLigneDto(
    int IntrantId,
    decimal Quantite);

public record CreateInterventionChargeDto(
    int TypeChargeId,
    string Libelle,
    DateTime Date,
    decimal MontantTtc,
    string? Note);

public record InterventionChargeListItemDto(
    int Id,
    int TypeChargeId,
    string TypeChargeNom,
    string Libelle,
    DateTime Date,
    decimal MontantTtc,
    string? Note);

public record InterventionDto(
    int Id,
    int SecteurId,
    string SecteurNom,
    DateTime Date,
    decimal? QuantiteEau,
    string? Note,
    decimal TotalCharges,
    IReadOnlyList<InterventionLigneDto> Lignes,
    IReadOnlyList<InterventionChargeListItemDto> Charges);

public record CreateInterventionDto(
    int SecteurId,
    DateTime Date,
    decimal? QuantiteEau,
    string? Note,
    IReadOnlyList<CreateInterventionLigneDto> Lignes,
    IReadOnlyList<CreateInterventionChargeDto> Charges);

public record UpdateInterventionDto(
    int SecteurId,
    DateTime Date,
    decimal? QuantiteEau,
    string? Note,
    IReadOnlyList<CreateInterventionLigneDto> Lignes,
    IReadOnlyList<CreateInterventionChargeDto> Charges);

public record InterventionListItemDto(
    int Id,
    int SecteurId,
    string SecteurNom,
    DateTime Date,
    string IntrantSummary,
    decimal? QuantiteEau,
    decimal TotalCharges,
    string? Note);
