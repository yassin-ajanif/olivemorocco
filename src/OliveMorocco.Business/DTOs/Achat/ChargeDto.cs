namespace OliveMorocco.Business.DTOs.Achat;

public record ChargeDto(
    int Id,
    int TypeChargeId,
    string TypeChargeNom,
    int? InterventionId,
    string Libelle,
    DateTime Date,
    decimal MontantTtc,
    string Note);

public record CreateChargeDto(
    int TypeChargeId,
    string Libelle,
    DateTime Date,
    decimal MontantTtc,
    string Note);

public record UpdateChargeDto(
    int TypeChargeId,
    string Libelle,
    DateTime Date,
    decimal MontantTtc,
    string Note);

public record ChargeListItemDto(
    int Id,
    string TypeChargeNom,
    string Libelle,
    DateTime Date,
    decimal MontantTtc,
    int? InterventionId);

public record TypeChargeSelectItemDto(
    int Id,
    string Nom);
