using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Domain.Entities.Vente;

public class BonCommandeClient : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public int? DevisId { get; set; }
    public int? FactureId { get; set; }
    public string Note { get; set; } = string.Empty;

    public Tiers Client { get; set; } = null!;
    public DevisClient? Devis { get; set; }
    public FactureClient? Facture { get; set; }
    public ICollection<BonCommandeClientLigne> Lignes { get; set; } = new List<BonCommandeClientLigne>();
    public ICollection<BonLivraisonClient> BonsLivraisonClient { get; set; } = new List<BonLivraisonClient>();
}
