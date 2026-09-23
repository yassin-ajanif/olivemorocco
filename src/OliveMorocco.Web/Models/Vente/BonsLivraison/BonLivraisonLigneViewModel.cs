namespace OliveMorocco.Web.Models.Vente.BonsLivraison;

public sealed class BonLivraisonLigneViewModel
{
    public int ProduitId { get; set; }

    public string Reference { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public decimal QuantiteCommandee { get; set; } = 1;

    public decimal QuantiteLivree { get; set; } = 1;

    public decimal PrixUnitaireHT { get; set; }

    public decimal Remise { get; set; }

    public decimal TauxTVA { get; set; } = 20;
}
