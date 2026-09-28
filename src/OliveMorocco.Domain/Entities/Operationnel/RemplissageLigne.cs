using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Domain.Entities.Operationnel;

public class RemplissageLigne : BaseEntity
{
    public int RemplissageId { get; set; }
    public int ProduitId { get; set; }
    public decimal Quantite { get; set; }
    public decimal ContenanceLitres { get; set; }
    public decimal Litres { get; set; }

    public Remplissage Remplissage { get; set; } = null!;
    public Produit Produit { get; set; } = null!;
}
