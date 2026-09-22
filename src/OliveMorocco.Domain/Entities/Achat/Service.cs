using OliveMorocco.Domain.Common;

namespace OliveMorocco.Domain.Entities.Achat;

public class Service : BaseEntity
{
    public string? Reference { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Unite { get; set; } = string.Empty;
    public decimal? PrixAchatHT { get; set; }
    public decimal TauxTVA { get; set; } = 20m;
    public bool Actif { get; set; } = true;

    public ICollection<BonCommandeFournisseurLigne> BonCommandeFournisseurLignes { get; set; } = new List<BonCommandeFournisseurLigne>();
    public ICollection<FactureFournisseurLigne> FactureFournisseurLignes { get; set; } = new List<FactureFournisseurLigne>();
}
