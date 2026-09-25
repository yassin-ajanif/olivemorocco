namespace OliveMorocco.Business.DTOs.Vente;

public record BonCommandeClientLigneDto(
    int Id,
    int BonCommandeClientId,
    int ProduitId,
    string Reference,
    string Designation,
    string Conditionnement,
    decimal QuantiteCommandee,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA);

public record CreateBonCommandeClientLigneDto(
    int ProduitId,
    string Designation,
    decimal QuantiteCommandee,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA,
    string? Conditionnement);

public record BonCommandeClientDto(
    int Id,
    string Numero,
    int ClientId,
    int? DevisId,
    int? FactureId,
    DateTime Date,
    decimal TotalTtc,
    string Note,
    IReadOnlyList<BonCommandeClientLigneDto> Lignes);

public record CreateBonCommandeClientDto(
    string Numero,
    int ClientId,
    int? DevisId,
    DateTime Date,
    string Note,
    IReadOnlyList<CreateBonCommandeClientLigneDto> Lignes);

public record UpdateBonCommandeClientDto(
    int ClientId,
    int? DevisId,
    DateTime Date,
    string Note,
    IReadOnlyList<CreateBonCommandeClientLigneDto> Lignes);

public record BonCommandeClientListItemDto(
    int Id,
    string Numero,
    int ClientId,
    string ClientNom,
    DateTime Date,
    decimal TotalTtc,
    string Note);
