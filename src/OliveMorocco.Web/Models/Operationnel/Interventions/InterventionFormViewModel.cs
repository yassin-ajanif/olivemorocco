using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Web.Models.Operationnel.Interventions;

public sealed class InterventionFormViewModel
{
    public int? Id { get; set; }

    public int SecteurId { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public int? IntrantId { get; set; }

    public decimal? QuantiteIntrant { get; set; }

    public decimal? QuantiteEau { get; set; }

    public string? Note { get; set; }

    public string? SecteurNom { get; set; }

    public decimal TotalCharges { get; set; }

    public IReadOnlyList<InterventionChargeListItemDto> LinkedCharges { get; set; } = [];

    public IReadOnlyList<SecteurSelectItemDto> Secteurs { get; set; } = [];

    public IReadOnlyList<IntrantSelectItemDto> Intrants { get; set; } = [];

    public bool IsEdit => Id.HasValue;

    public string? SelectedIntrantUnite =>
        IntrantId is null or 0
            ? null
            : Intrants.FirstOrDefault(i => i.Id == IntrantId)?.Unite;
}
