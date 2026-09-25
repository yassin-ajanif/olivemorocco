using OliveMorocco.Business.DTOs.Achat;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Web.Models.Operationnel.Interventions;

public sealed class InterventionFormViewModel
{
    public int? Id { get; set; }

    public int SecteurId { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public decimal? QuantiteEau { get; set; }

    public string? Note { get; set; }

    public string? SecteurNom { get; set; }

    public IList<InterventionLigneViewModel> Lignes { get; set; } = [];

    public IList<InterventionChargeViewModel> Charges { get; set; } = [];

    public IReadOnlyList<SecteurSelectItemDto> Secteurs { get; set; } = [];

    public IReadOnlyList<IntrantSelectItemDto> Intrants { get; set; } = [];

    public IReadOnlyList<TypeChargeSelectItemDto> TypeCharges { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
