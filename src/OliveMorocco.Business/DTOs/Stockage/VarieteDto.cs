namespace OliveMorocco.Business.DTOs.Stockage;

public record CreateVarieteDto(
    string Nom,
    string? Code,
    string? RegionOrigine);

public record VarieteCreatedDto(
    int Id,
    string Nom);
