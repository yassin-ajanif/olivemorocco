using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Operationnel;

public class InterventionLigne : BaseEntity
{
    public int InterventionId { get; set; }
    public int IntrantId { get; set; }
    public decimal Quantite { get; set; }

    public Intervention Intervention { get; set; } = null!;
    public Intrant Intrant { get; set; } = null!;
}
