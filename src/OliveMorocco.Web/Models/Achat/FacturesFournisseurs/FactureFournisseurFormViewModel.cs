namespace OliveMorocco.Web.Models.Achat.FacturesFournisseurs;

public sealed class FactureFournisseurFormViewModel
{
    public int? Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int FournisseurId { get; set; }

    public string FournisseurNom { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public DateTime DateEcheance { get; set; } = DateTime.Today.AddDays(30);

    public decimal RemiseGlobale { get; set; }

    public bool EstPayee { get; set; }

    public string? Note { get; set; }

    public List<FactureFournisseurLigneViewModel> Lignes { get; set; } = [];

    public List<LinkedBonReceptionViewModel> LinkedBonsReception { get; set; } = [];

    public bool LockFournisseur { get; set; }

    public bool IsEdit => Id.HasValue;
}
