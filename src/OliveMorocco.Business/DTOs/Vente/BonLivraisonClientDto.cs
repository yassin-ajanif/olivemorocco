namespace OliveMorocco.Business.DTOs.Vente;

public record BonLivraisonClientLigneDto(
    int Id,
    int BonLivraisonClientId,
    int ProduitId,
    string Reference,
    string Designation,
    decimal QuantiteCommandee,
    decimal QuantiteLivree,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA);

public record CreateBonLivraisonClientLigneDto(
    int ProduitId,
    string Designation,
    decimal QuantiteCommandee,
    decimal QuantiteLivree,
    decimal PrixUnitaireHT,
    decimal Remise,
    decimal TauxTVA);

public record BonLivraisonClientDto(
    int Id,
    string Numero,
    int ClientId,
    int? DevisId,
    int? BonCommandeClientId,
    int? FactureId,
    DateTime Date,
    decimal TotalTtc,
    string Note,
    IReadOnlyList<BonLivraisonClientLigneDto> Lignes);

public record CreateBonLivraisonClientDto(
    string Numero,
    int ClientId,
    int? DevisId,
    int? BonCommandeClientId,
    DateTime Date,
    string Note,
    IReadOnlyList<CreateBonLivraisonClientLigneDto> Lignes);

public record UpdateBonLivraisonClientDto(
    int ClientId,
    int? DevisId,
    int? BonCommandeClientId,
    DateTime Date,
    string Note,
    IReadOnlyList<CreateBonLivraisonClientLigneDto> Lignes);

public record BonLivraisonClientListItemDto(
    int Id,
    string Numero,
    int ClientId,
    string ClientNom,
    DateTime Date,
    decimal TotalTtc,
    string Note);
