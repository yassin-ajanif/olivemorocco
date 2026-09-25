namespace OliveMorocco.Business.DTOs.Vente;

public record FactureClientLigneDto(
    int Id,
    int FactureClientId,
    int? BonLivraisonId,
    int ProduitId,
    string Reference,
    string Designation,
    string Conditionnement,
    decimal Quantite,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA);

public record CreateFactureClientLigneDto(
    int? BonLivraisonId,
    int ProduitId,
    string Designation,
    decimal Quantite,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA,
    string? Conditionnement);

public record FactureClientDto(
    int Id,
    string Numero,
    int ClientId,
    int? DevisId,
    DateTime Date,
    DateTime DateEcheance,
    string BonCommandeReference,
    decimal RemiseGlobale,
    decimal TotalTtc,
    bool EstPayee,
    string Note,
    IReadOnlyList<FactureClientLigneDto> Lignes);

public record CreateFactureClientDto(
    string Numero,
    int ClientId,
    int? DevisId,
    DateTime Date,
    DateTime DateEcheance,
    string BonCommandeReference,
    decimal RemiseGlobale,
    decimal TotalTtc,
    bool EstPayee,
    string Note,
    IReadOnlyList<CreateFactureClientLigneDto> Lignes);

public record UpdateFactureClientDto(
    int ClientId,
    int? DevisId,
    DateTime Date,
    DateTime DateEcheance,
    string BonCommandeReference,
    decimal RemiseGlobale,
    decimal TotalTtc,
    bool EstPayee,
    string Note,
    IReadOnlyList<CreateFactureClientLigneDto> Lignes);

public record FactureClientListItemDto(
    int Id,
    string Numero,
    int ClientId,
    string ClientNom,
    DateTime Date,
    DateTime DateEcheance,
    decimal TotalTtc,
    bool EstPayee,
    string Note);
