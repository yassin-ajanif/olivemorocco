namespace OliveMorocco.Web.Models.Achat.AvoirFournisseur;

public sealed class AvoirFournisseurFormViewModel
{
    public int? Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int FournisseurId { get; set; }

    public string FournisseurNom { get; set; } = string.Empty;

    public int? FactureId { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public string Motif { get; set; } = string.Empty;

    public bool RetourMarchandise { get; set; }

    public List<AvoirFournisseurLigneViewModel> Lignes { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
