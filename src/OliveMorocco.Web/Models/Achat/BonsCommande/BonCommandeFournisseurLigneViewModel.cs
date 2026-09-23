namespace OliveMorocco.Web.Models.Achat.BonsCommande;

public sealed class BonCommandeFournisseurLigneViewModel
{
    public int ProduitId { get; set; }

    public string Reference { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public decimal QuantiteCommandee { get; set; } = 1;

    public string Unite { get; set; } = string.Empty;

    public decimal PrixUnitaireHT { get; set; }

    public decimal Remise { get; set; }

    public decimal TauxTVA { get; set; } = 20;
}
