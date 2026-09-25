namespace OliveMorocco.Web.Models.Vente.Devis;

public sealed class DevisFormViewModel
{
    public int? Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int ClientId { get; set; }

    public string ClientNom { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public DateTime DateValidite { get; set; } = DateTime.Today.AddDays(30);

    public decimal RemiseGlobale { get; set; }

    public string? Note { get; set; }

    public List<DevisLigneViewModel> Lignes { get; set; } = [];

    public bool IsEdit => Id.HasValue;
}
