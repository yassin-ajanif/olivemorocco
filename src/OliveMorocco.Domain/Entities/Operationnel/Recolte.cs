using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Operationnel;

public class Recolte : BaseEntity
{
    public int SecteurId { get; set; }
    public int VarieteId { get; set; }
    public DateTime Date { get; set; }
    public decimal Quantite { get; set; }

    public Secteur Secteur { get; set; } = null!;
    public Variete Variete { get; set; } = null!;
}
