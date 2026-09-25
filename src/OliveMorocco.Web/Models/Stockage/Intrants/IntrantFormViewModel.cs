namespace OliveMorocco.Web.Models.Stockage.Intrants;

public sealed class IntrantFormViewModel
{
    public int? Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Unite { get; set; } = string.Empty;

    public bool IsEdit => Id.HasValue;
}
