using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Web.Models.Shared;

public sealed class FacturePaiementViewModel
{
    public DateTime Date { get; set; } = DateTime.Today;

    public decimal Montant { get; set; }

    public ModePaiement Mode { get; set; } = ModePaiement.Virement;

    public string? Reference { get; set; }

    public bool EstEncaisse { get; set; } = true;
}
