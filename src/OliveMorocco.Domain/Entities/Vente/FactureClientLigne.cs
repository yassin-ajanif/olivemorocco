using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Vente;

public class FactureClientLigne : BaseEntity
{
    public int FactureClientId { get; set; }
    public int? BonLivraisonId { get; set; }
    public int ProduitId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Conditionnement { get; set; } = string.Empty;
    public decimal Quantite { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal Remise { get; set; }
    public decimal TauxTVA { get; set; }

    public FactureClient FactureClient { get; set; } = null!;
    public BonLivraisonClient? BonLivraison { get; set; }
    public Produit Produit { get; set; } = null!;
}
