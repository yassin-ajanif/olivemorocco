using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Domain.Entities.Achat;

public class FactureFournisseur : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int FournisseurId { get; set; }
    public DateTime Date { get; set; }
    public DateTime DateEcheance { get; set; }
    public decimal RemiseGlobale { get; set; }
    public decimal TotalTtc { get; set; }
    public bool EstPayee { get; set; }
    public string Note { get; set; } = string.Empty;

    public Tiers Fournisseur { get; set; } = null!;
    public ICollection<FactureFournisseurLigne> Lignes { get; set; } = new List<FactureFournisseurLigne>();
    public ICollection<PaiementFournisseur> Paiements { get; set; } = new List<PaiementFournisseur>();
    public ICollection<BonReception> BonsReception { get; set; } = new List<BonReception>();
    public ICollection<Pressage> Pressages { get; set; } = new List<Pressage>();
}
