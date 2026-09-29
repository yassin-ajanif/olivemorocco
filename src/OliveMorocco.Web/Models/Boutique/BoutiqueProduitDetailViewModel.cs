namespace OliveMorocco.Web.Models.Boutique;

public sealed class BoutiqueProduitDetailViewModel
{
    public BoutiqueProduitViewModel Produit { get; init; } = new();

    /// <summary>The variety this product belongs to — drives the hover badge on the detail page.</summary>
    public BoutiqueVarieteViewModel? Variete { get; init; }

    public string? Resume { get; init; }

    /// <summary>Label/value rows from the product spec list.</summary>
    public IReadOnlyList<(string Label, string Valeur)> Specs { get; init; } = [];

    /// <summary>Sibling formats of the same variety, shown under "Autres contenances".</summary>
    public IReadOnlyList<BoutiqueProduitViewModel> Autres { get; init; } = [];

    /// <summary>Products from the other varieties, shown under "Autres variétés".</summary>
    public IReadOnlyList<BoutiqueProduitViewModel> AutresVarietes { get; init; } = [];
}
