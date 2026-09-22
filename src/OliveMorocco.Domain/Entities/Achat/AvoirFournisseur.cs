using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Domain.Entities.Achat;

public class AvoirFournisseur : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int FournisseurId { get; set; }
    public DateTime Date { get; set; }
    public string Motif { get; set; } = string.Empty;
    public bool RetourMarchandise { get; set; }

    public Tiers Fournisseur { get; set; } = null!;
    public ICollection<AvoirFournisseurLigne> Lignes { get; set; } = new List<AvoirFournisseurLigne>();
}
