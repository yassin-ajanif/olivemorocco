namespace OliveMorocco.Web.Models.Vente.Avoirs;

public sealed class AvoirFormViewModel
{
    public int? Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int ClientId { get; set; }

    public string ClientNom { get; set; } = string.Empty;

    public int? FactureId { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;

    public string Motif { get; set; } = string.Empty;

    public bool RetourMarchandise { get; set; }

    public List<AvoirLigneViewModel> Lignes { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
