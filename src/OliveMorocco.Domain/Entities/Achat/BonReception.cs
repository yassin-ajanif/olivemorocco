using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Domain.Entities.Achat;

public class BonReception : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int FournisseurId { get; set; }
    public DateTime Date { get; set; }
    public int? BonCommandeId { get; set; }
    public int? FactureFournisseurId { get; set; }
    public decimal TotalTtc { get; set; }
    public string Note { get; set; } = string.Empty;

    public Tiers Fournisseur { get; set; } = null!;
    public BonCommandeFournisseur? BonCommande { get; set; }
    public FactureFournisseur? FactureFournisseur { get; set; }
    public ICollection<BonReceptionLigne> Lignes { get; set; } = new List<BonReceptionLigne>();
    public ICollection<FactureFournisseurLigne> FactureFournisseurLignes { get; set; } = new List<FactureFournisseurLigne>();
}
