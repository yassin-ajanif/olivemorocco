using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Achat;

namespace OliveMorocco.Domain.Entities.Operationnel;

public class Intervention : BaseEntity
{
    public int SecteurId { get; set; }
    public DateTime Date { get; set; }
    public int? IntrantId { get; set; }
    public decimal? QuantiteIntrant { get; set; }
    public decimal? QuantiteEau { get; set; }
    public string? Note { get; set; }

    public Secteur Secteur { get; set; } = null!;
    public Intrant? Intrant { get; set; }
    public ICollection<Charge> Charges { get; set; } = new List<Charge>();
}
