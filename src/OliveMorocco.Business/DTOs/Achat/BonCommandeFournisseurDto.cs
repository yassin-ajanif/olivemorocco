namespace OliveMorocco.Business.DTOs.Achat;

public record BonCommandeFournisseurLigneDto(
    int Id,
    int BonCommandeFournisseurId,
    int? IntrantId,
    int? ServiceId,
    string Reference,
    string Designation,
    string Conditionnement,
    decimal QuantiteCommandee,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA);

public record CreateBonCommandeFournisseurLigneDto(
    int? IntrantId,
    int? ServiceId,
    string Designation,
    decimal QuantiteCommandee,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA,
    string? Conditionnement);

public record BonCommandeFournisseurDto(
    int Id,
    string Numero,
    int FournisseurId,
    DateTime Date,
    decimal TotalTtc,
    string Note,
    IReadOnlyList<BonCommandeFournisseurLigneDto> Lignes);

public record CreateBonCommandeFournisseurDto(
    string Numero,
    int FournisseurId,
    DateTime Date,
    string Note,
    IReadOnlyList<CreateBonCommandeFournisseurLigneDto> Lignes);

public record UpdateBonCommandeFournisseurDto(
    int FournisseurId,
    DateTime Date,
    string Note,
    IReadOnlyList<CreateBonCommandeFournisseurLigneDto> Lignes);

public record BonCommandeFournisseurListItemDto(
    int Id,
    string Numero,
    int FournisseurId,
    string FournisseurNom,
    DateTime Date,
    decimal TotalTtc,
    string Note);
