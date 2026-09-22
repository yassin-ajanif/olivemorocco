using OliveMorocco.Domain.Common;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Domain.Entities.Vente;

public class Produit : BaseEntity
{
    public string Reference { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int? VarieteId { get; set; }
    public string Unite { get; set; } = string.Empty;
    public string? CodeBarre { get; set; }
    public decimal PrixAchatHT { get; set; }
    public decimal PrixVenteHT { get; set; }
    public decimal TauxTVA { get; set; }
    public decimal StockActuel { get; set; }
    public decimal StockMinimum { get; set; }
    public bool Actif { get; set; } = true;
    public byte[]? ImageData { get; set; }

    public Variete? Variete { get; set; }
    public ICollection<MouvementStock> MouvementsStock { get; set; } = new List<MouvementStock>();
    public ICollection<DevisClientLigne> DevisClientLignes { get; set; } = new List<DevisClientLigne>();
    public ICollection<BonCommandeClientLigne> BonCommandeClientLignes { get; set; } = new List<BonCommandeClientLigne>();
    public ICollection<BonLivraisonClientLigne> BonLivraisonClientLignes { get; set; } = new List<BonLivraisonClientLigne>();
    public ICollection<FactureClientLigne> FactureClientLignes { get; set; } = new List<FactureClientLigne>();
    public ICollection<AvoirClientLigne> AvoirClientLignes { get; set; } = new List<AvoirClientLigne>();
}
