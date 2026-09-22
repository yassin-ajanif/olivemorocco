using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Vente;

public class BonLivraisonClientLigne : BaseEntity
{
    public int BLId { get; set; }
    public int ProduitId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public decimal QuantiteCommandee { get; set; }
    public decimal QuantiteLivree { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal Remise { get; set; }
    public decimal TauxTVA { get; set; }

    public BonLivraisonClient BonLivraisonClient { get; set; } = null!;
    public Produit Produit { get; set; } = null!;
}
