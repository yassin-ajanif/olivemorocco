using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Vente;

namespace OliveMorocco.Domain.Entities.Operationnel;

public class Variete : BaseEntity
{
    public string Nom { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? RegionOrigine { get; set; }

    public ICollection<SecteurVariete> SecteurVarietes { get; set; } = new List<SecteurVariete>();
    public ICollection<Produit> Produits { get; set; } = new List<Produit>();
    public ICollection<Recolte> Recoltes { get; set; } = new List<Recolte>();
    public ICollection<Pressage> Pressages { get; set; } = new List<Pressage>();
}
