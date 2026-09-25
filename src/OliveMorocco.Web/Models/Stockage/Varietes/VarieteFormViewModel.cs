namespace OliveMorocco.Web.Models.Stockage.Varietes;

public sealed class VarieteFormViewModel
{
    public int? Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string? RegionOrigine { get; set; }

    public bool IsEdit => Id.HasValue;
}
