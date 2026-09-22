using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Domain.Entities.Vente;

public class MouvementStock : BaseEntity
{
    public int ProduitId { get; set; }
    public TypeMouvement Type { get; set; }
    public decimal Quantite { get; set; }
    public decimal StockAvant { get; set; }
    public string OrigineType { get; set; } = string.Empty;
    public int? OrigineId { get; set; }
    public string Note { get; set; } = string.Empty;

    public Produit Produit { get; set; } = null!;
}
