namespace OliveMorocco.Business.DTOs.Stockage;

public record IntrantDto(
    int Id,
    string Nom,
    string Unite,
    decimal PrixAchatHT);

public record CreateIntrantDto(
    string Nom,
    string Unite,
    decimal PrixAchatHT);

public record UpdateIntrantDto(
    string Nom,
    string Unite,
    decimal PrixAchatHT);

public record IntrantListItemDto(
    int Id,
    string Nom,
    string Unite,
    decimal PrixAchatHT);
