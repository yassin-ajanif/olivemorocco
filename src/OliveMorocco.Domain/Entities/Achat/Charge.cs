using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Domain.Entities.Achat;

public class Charge : BaseEntity
{
    public int TypeChargeId { get; set; }
    public int? InterventionId { get; set; }
    public string Libelle { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal MontantTtc { get; set; }
    public string Note { get; set; } = string.Empty;

    public TypeCharge TypeCharge { get; set; } = null!;
    public Intervention? Intervention { get; set; }
}
