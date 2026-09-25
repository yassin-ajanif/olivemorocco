namespace OliveMorocco.Web.Models.Achat.BonsReception;

public sealed class BonReceptionFormViewModel
{
    public int? Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int FournisseurId { get; set; }

    public string FournisseurNom { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public string? Note { get; set; }

    public List<BonReceptionLigneViewModel> Lignes { get; set; } = [];

    public int? FactureId { get; set; }

    public string? FactureNumero { get; set; }

    public bool IsEdit => Id.HasValue;

    public bool IsFactured => FactureId is > 0;
}
