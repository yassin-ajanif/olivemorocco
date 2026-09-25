using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Domain.Entities.Achat;

public class BonReceptionLigne : BaseEntity
{
    public int BRId { get; set; }
    public int IntrantId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public decimal QuantiteRecue { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal TauxTVA { get; set; }

    public BonReception BonReception { get; set; } = null!;
    public Intrant Intrant { get; set; } = null!;
}
