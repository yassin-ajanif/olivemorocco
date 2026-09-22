using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Domain.Entities.Achat;

public class PaiementFournisseur : BaseEntity
{
    public int FactureFournisseurId { get; set; }
    public DateTime Date { get; set; }
    public decimal Montant { get; set; }
    public ModePaiement Mode { get; set; }
    public string Reference { get; set; } = string.Empty;
    public bool EstEncaisse { get; set; }

    public FactureFournisseur FactureFournisseur { get; set; } = null!;
}
