using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Domain.Entities.Vente;

public class BonLivraisonClient : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public int? DevisId { get; set; }
    public int? BonCommandeClientId { get; set; }
    public int? FactureId { get; set; }
    public string Note { get; set; } = string.Empty;

    public Tiers Client { get; set; } = null!;
    public DevisClient? Devis { get; set; }
    public BonCommandeClient? BonCommandeClient { get; set; }
    public FactureClient? Facture { get; set; }
    public ICollection<BonLivraisonClientLigne> Lignes { get; set; } = new List<BonLivraisonClientLigne>();
    public ICollection<FactureClientLigne> FactureClientLignes { get; set; } = new List<FactureClientLigne>();
}
