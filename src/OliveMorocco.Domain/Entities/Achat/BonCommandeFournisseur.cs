using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Common;

namespace OliveMorocco.Domain.Entities.Achat;

public class BonCommandeFournisseur : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int FournisseurId { get; set; }
    public DateTime Date { get; set; }
    public string Note { get; set; } = string.Empty;

    public Tiers Fournisseur { get; set; } = null!;
    public ICollection<BonCommandeFournisseurLigne> Lignes { get; set; } = new List<BonCommandeFournisseurLigne>();
    public ICollection<BonReception> BonsReception { get; set; } = new List<BonReception>();
}
