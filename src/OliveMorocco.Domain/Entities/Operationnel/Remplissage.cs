using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Operationnel;

public class Remplissage : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int VarieteId { get; set; }
    public DateTime Date { get; set; }
    public decimal QuantiteHuile { get; set; }
    public decimal Perte { get; set; }
    public string? Note { get; set; }

    public Variete Variete { get; set; } = null!;
    public ICollection<RemplissageLigne> Lignes { get; set; } = new List<RemplissageLigne>();
}
