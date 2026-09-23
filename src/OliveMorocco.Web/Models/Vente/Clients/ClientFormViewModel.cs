namespace OliveMorocco.Web.Models.Vente.Clients;

public sealed class ClientFormViewModel
{
    public int? Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string? ICE { get; set; }

    public string? Adresse { get; set; }

    public string? Ville { get; set; }

    public string? Telephone { get; set; }

    public string? Email { get; set; }

    public string? ConditionsPaiement { get; set; }

    public bool Actif { get; set; } = true;

    public bool IsEdit => Id.HasValue;
}
