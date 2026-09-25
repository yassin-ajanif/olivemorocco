namespace OliveMorocco.Business.DTOs.Stockage;

public record SecteurVarieteLineDto(
    int Id,
    int SecteurId,
    int VarieteId,
    string VarieteNom,
    decimal SuperficieHectares);

public record CreateSecteurVarieteLineDto(
    int VarieteId,
    decimal SuperficieHectares);

public record SecteurDto(
    int Id,
    string Nom,
    string? Code,
    decimal SuperficieHectares,
    IReadOnlyList<SecteurVarieteLineDto> Lignes);

public record CreateSecteurDto(
    string Nom,
    string? Code,
    decimal SuperficieHectares,
    IReadOnlyList<CreateSecteurVarieteLineDto> Lignes);

public record UpdateSecteurDto(
    string Nom,
    string? Code,
    decimal SuperficieHectares,
    IReadOnlyList<CreateSecteurVarieteLineDto> Lignes);

public record SecteurListItemDto(
    int Id,
    string Nom,
    string? Code,
    decimal SuperficieHectares,
    string VarieteSummary);
