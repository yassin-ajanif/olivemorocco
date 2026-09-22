using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Vente;

public class BonCommandeClientLigne : BaseEntity
{
    public int BonCommandeClientId { get; set; }
    public int ProduitId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Conditionnement { get; set; } = string.Empty;
    public decimal QuantiteCommandee { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal Remise { get; set; }
    public decimal TauxTVA { get; set; }

    public BonCommandeClient BonCommandeClient { get; set; } = null!;
    public Produit Produit { get; set; } = null!;
}
