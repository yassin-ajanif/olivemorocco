namespace OliveMorocco.Business.DTOs.Achat;

public record AvoirFournisseurLigneDto(
    int Id,
    int AvoirFournisseurId,
    int ProduitId,
    string Reference,
    string Designation,
    string Conditionnement,
    decimal Quantite,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA);

public record CreateAvoirFournisseurLigneDto(
    int ProduitId,
    string Designation,
    decimal Quantite,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA,
    string? Conditionnement);

public record AvoirFournisseurDto(
    int Id,
    string Numero,
    int FournisseurId,
    int? FactureFournisseurId,
    DateTime Date,
    string Motif,
    bool RetourMarchandise,
    decimal TotalTtc,
    IReadOnlyList<AvoirFournisseurLigneDto> Lignes);

public record CreateAvoirFournisseurDto(
    string Numero,
    int FournisseurId,
    int? FactureFournisseurId,
    DateTime Date,
    string Motif,
    bool RetourMarchandise,
    IReadOnlyList<CreateAvoirFournisseurLigneDto> Lignes);

public record UpdateAvoirFournisseurDto(
    int FournisseurId,
    int? FactureFournisseurId,
    DateTime Date,
    string Motif,
    bool RetourMarchandise,
    IReadOnlyList<CreateAvoirFournisseurLigneDto> Lignes);

public record AvoirFournisseurListItemDto(
    int Id,
    string Numero,
    int FournisseurId,
    string FournisseurNom,
    DateTime Date,
    decimal TotalTtc,
    string Motif,
    bool RetourMarchandise);
