using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Operationnel;

public class Secteur : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string? Code { get; set; }
    public decimal SuperficieHectares { get; set; }

    public ICollection<SecteurVariete> SecteurVarietes { get; set; } = new List<SecteurVariete>();
    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();
    public ICollection<Recolte> Recoltes { get; set; } = new List<Recolte>();
}
