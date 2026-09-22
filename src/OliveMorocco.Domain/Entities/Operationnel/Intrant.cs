using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Operationnel;

public class Intrant : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string Unite { get; set; } = string.Empty;

    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();
}
