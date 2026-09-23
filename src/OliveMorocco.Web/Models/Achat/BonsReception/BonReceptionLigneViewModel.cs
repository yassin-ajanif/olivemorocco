namespace OliveMorocco.Web.Models.Achat.BonsReception;

public sealed class BonReceptionLigneViewModel
{
    public int ProduitId { get; set; }

    public string Reference { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public decimal QuantiteCommandee { get; set; } = 1;

    public decimal QuantiteRecue { get; set; } = 1;

    public decimal PrixUnitaireHT { get; set; }

    public decimal Remise { get; set; }

    public decimal TauxTVA { get; set; } = 20;
}
