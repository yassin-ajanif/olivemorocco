namespace OliveMorocco.Web.Models.Vente.Clients;

public sealed class ClientFormViewModel
{
    public int? Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string ICE { get; set; } = string.Empty;

    public string Adresse { get; set; } = string.Empty;

    public string Ville { get; set; } = string.Empty;

    public string Telephone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string ConditionsPaiement { get; set; } = string.Empty;

    public bool Actif { get; set; } = true;

    public bool IsEdit => Id.HasValue;
}
