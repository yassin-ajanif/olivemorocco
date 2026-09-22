using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Domain.Entities.Operationnel;

public class Pressage : BaseEntity
{
    public int FournisseurId { get; set; }
    public int VarieteId { get; set; }
    public DateTime Date { get; set; }
    public decimal QuantiteOlives { get; set; }
    public decimal Rendement { get; set; }
    public decimal? QuantiteHuile { get; set; }
    public int? FactureFournisseurId { get; set; }

    public Tiers Fournisseur { get; set; } = null!;
    public Variete Variete { get; set; } = null!;
    public FactureFournisseur? FactureFournisseur { get; set; }
}
