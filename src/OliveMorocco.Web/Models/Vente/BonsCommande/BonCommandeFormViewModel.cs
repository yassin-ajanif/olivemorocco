namespace OliveMorocco.Web.Models.Vente.BonsCommande;

public sealed class BonCommandeFormViewModel
{
    public int? Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int ClientId { get; set; }

    public string ClientNom { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public string? Note { get; set; }

    public List<BonCommandeLigneViewModel> Lignes { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
