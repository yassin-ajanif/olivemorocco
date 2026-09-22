using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Domain.Entities.Vente;

public class PaiementClient : BaseEntity
{
    public int FactureClientId { get; set; }
    public DateTime Date { get; set; }
    public decimal Montant { get; set; }
    public ModePaiement Mode { get; set; }
    public string Reference { get; set; } = string.Empty;
    public bool EstEncaisse { get; set; }

    public FactureClient FactureClient { get; set; } = null!;
}
