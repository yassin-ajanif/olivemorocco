using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Vente;

public class AvoirClientLigne : BaseEntity
{
    public int AvoirClientId { get; set; }
    public int ProduitId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Conditionnement { get; set; } = string.Empty;
    public decimal Quantite { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal Remise { get; set; }
    public decimal TauxTVA { get; set; }

    public AvoirClient AvoirClient { get; set; } = null!;
    public Produit Produit { get; set; } = null!;
}
