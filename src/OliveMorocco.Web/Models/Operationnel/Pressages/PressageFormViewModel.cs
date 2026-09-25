using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Web.Models.Operationnel.Pressages;

public sealed class PressageFormViewModel
{
    public int? Id { get; set; }

    public int FournisseurId { get; set; }

    public int VarieteId { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public decimal QuantiteOlives { get; set; }

    public decimal Rendement { get; set; }

    public decimal? QuantiteHuile { get; set; }

    public int? FactureFournisseurId { get; set; }

    public IReadOnlyList<FournisseurSelectItemDto> Fournisseurs { get; set; } = [];

    public IReadOnlyList<VarieteSelectItemDto> Varietes { get; set; } = [];

    public IReadOnlyList<FactureFournisseurSelectItemDto> Factures { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
