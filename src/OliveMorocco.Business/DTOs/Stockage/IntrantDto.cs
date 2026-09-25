namespace OliveMorocco.Business.DTOs.Stockage;

public record IntrantDto(
    int Id,
    string Nom,
    string Unite);

public record CreateIntrantDto(
    string Nom,
    string Unite);

public record UpdateIntrantDto(
    string Nom,
    string Unite);

public record IntrantListItemDto(
    int Id,
    string Nom,
    string Unite);
