using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Domain.Entities.Achat;

public class FactureFournisseurLigne : BaseEntity
{
    public int FactureFournisseurId { get; set; }
    public int? BonReceptionId { get; set; }
    public int? IntrantId { get; set; }
    public int? ServiceId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Conditionnement { get; set; } = string.Empty;
    public decimal Quantite { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal Remise { get; set; }
    public decimal TauxTVA { get; set; }

    public FactureFournisseur FactureFournisseur { get; set; } = null!;
    public BonReception? BonReception { get; set; }
    public Intrant? Intrant { get; set; }
    public Service? Service { get; set; }
}
