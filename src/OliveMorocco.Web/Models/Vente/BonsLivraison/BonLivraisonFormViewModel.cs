namespace OliveMorocco.Web.Models.Vente.BonsLivraison;

public sealed class BonLivraisonFormViewModel
{
    public int? Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int ClientId { get; set; }

    public string ClientNom { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public string? Note { get; set; }

    public List<BonLivraisonLigneViewModel> Lignes { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
