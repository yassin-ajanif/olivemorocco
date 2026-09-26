using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.DTOs.Common;

public record FacturePaiementDto(
    int Id,
    DateTime Date,
    decimal Montant,
    ModePaiement Mode,
    string Reference,
    bool EstEncaisse);

public record CreateFacturePaiementDto(
    DateTime Date,
    decimal Montant,
    ModePaiement Mode,
    string Reference,
    bool EstEncaisse);
