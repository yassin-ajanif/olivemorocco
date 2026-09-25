namespace OliveMorocco.Business.DTOs.Vente;

public record AvoirClientLigneDto(
    int Id,
    int AvoirClientId,
    int ProduitId,
    string Reference,
    string Designation,
    string Conditionnement,
    decimal Quantite,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA);

public record CreateAvoirClientLigneDto(
    int ProduitId,
    string Designation,
    decimal Quantite,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA,
    string? Conditionnement);

public record AvoirClientDto(
    int Id,
    string Numero,
    int ClientId,
    int? FactureId,
    DateTime Date,
    string Motif,
    bool RetourMarchandise,
    decimal TotalTtc,
    IReadOnlyList<AvoirClientLigneDto> Lignes);

public record CreateAvoirClientDto(
    string Numero,
    int ClientId,
    int? FactureId,
    DateTime Date,
    string Motif,
    bool RetourMarchandise,
    IReadOnlyList<CreateAvoirClientLigneDto> Lignes);

public record UpdateAvoirClientDto(
    int ClientId,
    int? FactureId,
    DateTime Date,
    string Motif,
    bool RetourMarchandise,
    IReadOnlyList<CreateAvoirClientLigneDto> Lignes);

public record AvoirClientListItemDto(
    int Id,
    string Numero,
    int ClientId,
    string ClientNom,
    DateTime Date,
    decimal TotalTtc,
    string Motif,
    bool RetourMarchandise);
