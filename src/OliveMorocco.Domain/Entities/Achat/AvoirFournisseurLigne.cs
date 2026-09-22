using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Domain.Entities.Achat;

public class AvoirFournisseurLigne : BaseEntity
{
    public int AvoirFournisseurId { get; set; }
    public int ProduitId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Conditionnement { get; set; } = string.Empty;
    public decimal Quantite { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal Remise { get; set; }
    public decimal TauxTVA { get; set; }

    public AvoirFournisseur AvoirFournisseur { get; set; } = null!;
    public Produit Produit { get; set; } = null!;
}
