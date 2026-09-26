using OliveMorocco.Web.Models.Shared;

namespace OliveMorocco.Web.Models.Vente.Facturation;

public sealed class FactureFormViewModel
{
    public int? Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int ClientId { get; set; }

    public string ClientNom { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public DateTime DateEcheance { get; set; } = DateTime.Today.AddDays(30);

    public string? BonCommandeReference { get; set; }

    public decimal RemiseGlobale { get; set; }

    public string? Note { get; set; }

    public List<FactureLigneViewModel> Lignes { get; set; } = [];

    public List<FacturePaiementViewModel> Paiements { get; set; } = [];

    public List<LinkedBonLivraisonViewModel> LinkedBonsLivraison { get; set; } = [];

    public bool LockClient { get; set; }

    public bool IsEdit => Id.HasValue;
}
