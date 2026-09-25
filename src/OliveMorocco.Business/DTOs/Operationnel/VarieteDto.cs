namespace OliveMorocco.Business.DTOs.Operationnel;

public record CreateVarieteDto(
    string Nom,
    string? Code,
    string? RegionOrigine);

public record VarieteCreatedDto(
    int Id,
    string Nom);
