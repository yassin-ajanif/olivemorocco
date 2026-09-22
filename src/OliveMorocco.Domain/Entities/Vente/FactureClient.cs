using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Domain.Entities.Vente;

public class FactureClient : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime Date { get; set; }
    public DateTime DateEcheance { get; set; }
    public int? DevisId { get; set; }
    public string BonCommandeReference { get; set; } = string.Empty;
    public decimal RemiseGlobale { get; set; }
    public decimal TotalTtc { get; set; }
    public bool EstPayee { get; set; }
    public string Note { get; set; } = string.Empty;

    public Tiers Client { get; set; } = null!;
    public DevisClient? Devis { get; set; }
    public ICollection<FactureClientLigne> Lignes { get; set; } = new List<FactureClientLigne>();
    public ICollection<PaiementClient> Paiements { get; set; } = new List<PaiementClient>();
    public ICollection<BonCommandeClient> BonsCommandeClient { get; set; } = new List<BonCommandeClient>();
    public ICollection<BonLivraisonClient> BonsLivraisonClient { get; set; } = new List<BonLivraisonClient>();
    public ICollection<AvoirClient> AvoirsClient { get; set; } = new List<AvoirClient>();
}
