using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Vente;

public class DevisClientLigne : BaseEntity
{
    public int DevisClientId { get; set; }
    public int ProduitId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Conditionnement { get; set; } = string.Empty;
    public decimal Quantite { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal Remise { get; set; }
    public decimal TauxTVA { get; set; }

    public DevisClient DevisClient { get; set; } = null!;
    public Produit Produit { get; set; } = null!;
}
