namespace OliveMorocco.Web.Models.Operationnel.Interventions;

public sealed class InterventionChargeViewModel
{
    public int TypeChargeId { get; set; }

    public string Libelle { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public decimal MontantTtc { get; set; }

    public string? Note { get; set; }
}
