using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Domain.Entities.Achat;

public class BonCommandeFournisseurLigne : BaseEntity
{
    public int BonCommandeFournisseurId { get; set; }
    public int? ProduitId { get; set; }
    public int? ServiceId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Conditionnement { get; set; } = string.Empty;
    public decimal QuantiteCommandee { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal Remise { get; set; }
    public decimal TauxTVA { get; set; }

    public BonCommandeFournisseur BonCommandeFournisseur { get; set; } = null!;
    public Produit? Produit { get; set; }
    public Service? Service { get; set; }
}
