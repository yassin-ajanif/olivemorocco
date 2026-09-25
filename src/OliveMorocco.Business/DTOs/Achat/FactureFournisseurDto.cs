namespace OliveMorocco.Business.DTOs.Achat;

public record FactureFournisseurLigneDto(
    int Id,
    int FactureFournisseurId,
    int? BonReceptionId,
    int? IntrantId,
    int? ServiceId,
    string Reference,
    string Designation,
    string Conditionnement,
    decimal Quantite,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA);

public record CreateFactureFournisseurLigneDto(
    int? BonReceptionId,
    int? IntrantId,
    int? ServiceId,
    string Designation,
    decimal Quantite,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA,
    string? Conditionnement);

public record FactureFournisseurDto(
    int Id,
    string Numero,
    int FournisseurId,
    DateTime Date,
    DateTime DateEcheance,
    decimal RemiseGlobale,
    decimal TotalTtc,
    bool EstPayee,
    string Note,
    IReadOnlyList<FactureFournisseurLigneDto> Lignes);

public record CreateFactureFournisseurDto(
    string Numero,
    int FournisseurId,
    DateTime Date,
    DateTime DateEcheance,
    decimal RemiseGlobale,
    decimal TotalTtc,
    bool EstPayee,
    string Note,
    IReadOnlyList<CreateFactureFournisseurLigneDto> Lignes);

public record UpdateFactureFournisseurDto(
    int FournisseurId,
    DateTime Date,
    DateTime DateEcheance,
    decimal RemiseGlobale,
    decimal TotalTtc,
    bool EstPayee,
    string Note,
    IReadOnlyList<CreateFactureFournisseurLigneDto> Lignes);

public record FactureFournisseurListItemDto(
    int Id,
    string Numero,
    int FournisseurId,
    string FournisseurNom,
    DateTime Date,
    DateTime DateEcheance,
    decimal TotalTtc,
    bool EstPayee,
    string Note);
