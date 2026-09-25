namespace OliveMorocco.Business.DTOs.Vente;

public record DevisClientLigneDto(
    int Id,
    int DevisClientId,
    int ProduitId,
    string Reference,
    string Designation,
    string Conditionnement,
    decimal Quantite,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA);

public record CreateDevisClientLigneDto(
    int ProduitId,
    string Designation,
    decimal Quantite,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA,
    string? Conditionnement);

public record DevisClientDto(
    int Id,
    string Numero,
    int ClientId,
    DateTime Date,
    DateTime DateValidite,
    decimal RemiseGlobale,
    string Note,
    decimal TotalTtc,
    IReadOnlyList<DevisClientLigneDto> Lignes);

public record CreateDevisClientDto(
    string Numero,
    int ClientId,
    DateTime Date,
    DateTime DateValidite,
    decimal RemiseGlobale,
    string Note,
    IReadOnlyList<CreateDevisClientLigneDto> Lignes);

public record UpdateDevisClientDto(
    int ClientId,
    DateTime Date,
    DateTime DateValidite,
    decimal RemiseGlobale,
    string Note,
    IReadOnlyList<CreateDevisClientLigneDto> Lignes);

public record DevisClientListItemDto(
    int Id,
    string Numero,
    int ClientId,
    string ClientNom,
    DateTime Date,
    DateTime DateValidite,
    decimal TotalTtc,
    string Note);

public record ArticleSuggestionDto(
    int ProduitId,
    string Reference,
    string Designation,
    string Unite,
    decimal PrixUnitaireHT,
    decimal TauxTVA);
