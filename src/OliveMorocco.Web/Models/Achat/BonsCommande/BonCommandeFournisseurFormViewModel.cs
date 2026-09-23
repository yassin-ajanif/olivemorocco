namespace OliveMorocco.Web.Models.Achat.BonsCommande;

public sealed class BonCommandeFournisseurFormViewModel
{
    public int? Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int FournisseurId { get; set; }

    public string FournisseurNom { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public string? Note { get; set; }

    public List<BonCommandeFournisseurLigneViewModel> Lignes { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
