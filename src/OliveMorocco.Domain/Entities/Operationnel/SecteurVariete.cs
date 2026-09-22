using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Operationnel;

public class SecteurVariete : BaseEntity
{
    public int SecteurId { get; set; }
    public int VarieteId { get; set; }
    public decimal SuperficieHectares { get; set; }

    public Secteur Secteur { get; set; } = null!;
    public Variete Variete { get; set; } = null!;
}
