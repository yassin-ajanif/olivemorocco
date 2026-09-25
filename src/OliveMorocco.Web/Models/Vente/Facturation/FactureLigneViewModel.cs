namespace OliveMorocco.Web.Models.Vente.Facturation;

public sealed class FactureLigneViewModel
{
    public int? BonLivraisonId { get; set; }

    public int ProduitId { get; set; }

    public string Reference { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public decimal Quantite { get; set; } = 1;

    public string Unite { get; set; } = string.Empty;

    public decimal PrixUnitaireHT { get; set; }

    public decimal Remise { get; set; }

    public decimal TauxTVA { get; set; } = 20;
}
