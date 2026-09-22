using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Entities.Vente;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Domain.Entities.Common;

public class Tiers : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public TypeTiers Type { get; set; }
    public string Adresse { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ICE { get; set; } = string.Empty;
    public string ConditionsPaiement { get; set; } = string.Empty;
    public bool Actif { get; set; } = true;

    public ICollection<DevisClient> DevisClients { get; set; } = new List<DevisClient>();
    public ICollection<BonCommandeClient> BonsCommandeClient { get; set; } = new List<BonCommandeClient>();
    public ICollection<BonLivraisonClient> BonsLivraisonClient { get; set; } = new List<BonLivraisonClient>();
    public ICollection<FactureClient> FacturesClient { get; set; } = new List<FactureClient>();
    public ICollection<AvoirClient> AvoirsClient { get; set; } = new List<AvoirClient>();
    public ICollection<BonCommandeFournisseur> BonsCommandeFournisseur { get; set; } = new List<BonCommandeFournisseur>();
    public ICollection<BonReception> BonsReception { get; set; } = new List<BonReception>();
    public ICollection<FactureFournisseur> FacturesFournisseur { get; set; } = new List<FactureFournisseur>();
    public ICollection<AvoirFournisseur> AvoirsFournisseur { get; set; } = new List<AvoirFournisseur>();
    public ICollection<Pressage> Pressages { get; set; } = new List<Pressage>();
}
