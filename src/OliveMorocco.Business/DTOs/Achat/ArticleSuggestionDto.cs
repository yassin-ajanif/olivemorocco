namespace OliveMorocco.Business.DTOs.Achat;

public record ArticleSuggestionDto(
    int ProduitId,
    string Reference,
    string Designation,
    string Unite,
    decimal PrixUnitaireHT,
    decimal TauxTVA);
