namespace OliveMorocco.Business.DTOs.Achat;

public record BonReceptionLigneDto(
    int Id,
    int BonReceptionId,
    int ProduitId,
    string Reference,
    string Designation,
    decimal QuantiteRecue,
    decimal PrixUnitaireHT,
    decimal TauxTVA);

public record CreateBonReceptionLigneDto(
    int ProduitId,
    string Designation,
    decimal QuantiteRecue,
    decimal PrixUnitaireHT,
    decimal TauxTVA);

public record BonReceptionDto(
    int Id,
    string Numero,
    int FournisseurId,
    int? BonCommandeId,
    int? FactureFournisseurId,
    string? FactureNumero,
    DateTime Date,
    decimal TotalTtc,
    string Note,
    IReadOnlyList<BonReceptionLigneDto> Lignes);

public record CreateBonReceptionDto(
    string Numero,
    int FournisseurId,
    int? BonCommandeId,
    DateTime Date,
    string Note,
    IReadOnlyList<CreateBonReceptionLigneDto> Lignes);

public record UpdateBonReceptionDto(
    int FournisseurId,
    int? BonCommandeId,
    DateTime Date,
    string Note,
    IReadOnlyList<CreateBonReceptionLigneDto> Lignes);

public record BonReceptionListItemDto(
    int Id,
    string Numero,
    int FournisseurId,
    string FournisseurNom,
    DateTime Date,
    decimal TotalTtc,
    string Note,
    int? FactureId,
    string? FactureNumero);
