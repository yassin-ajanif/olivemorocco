using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Domain.Entities.Vente;

public class DevisClient : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public DateTime DateValidite { get; set; }
    public decimal RemiseGlobale { get; set; }
    public string Note { get; set; } = string.Empty;

    public Tiers Client { get; set; } = null!;
    public ICollection<DevisClientLigne> Lignes { get; set; } = new List<DevisClientLigne>();
    public ICollection<BonCommandeClient> BonsCommandeClient { get; set; } = new List<BonCommandeClient>();
    public ICollection<BonLivraisonClient> BonsLivraisonClient { get; set; } = new List<BonLivraisonClient>();
    public ICollection<FactureClient> FacturesClient { get; set; } = new List<FactureClient>();
}
