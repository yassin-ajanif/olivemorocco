using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Domain.Entities.Achat;

public class BonReceptionLigne : BaseEntity
{
    public int BRId { get; set; }
    public int ProduitId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public decimal QuantiteRecue { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal TauxTVA { get; set; }

    public BonReception BonReception { get; set; } = null!;
    public Produit Produit { get; set; } = null!;
}
