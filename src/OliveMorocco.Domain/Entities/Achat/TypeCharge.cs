using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Achat;

public class TypeCharge : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public bool Actif { get; set; } = true;

    public ICollection<Charge> Charges { get; set; } = new List<Charge>();
}
