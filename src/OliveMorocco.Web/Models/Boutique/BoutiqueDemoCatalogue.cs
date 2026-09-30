using OliveMorocco.Web.Models.Boutique;

namespace OliveMorocco.Web.Models.Boutique;

/// <summary>
/// TEMPORARY: in-memory catalogue used to preview the shop UI.
/// <para>
/// Three varieties (Picholine, Haouzia, Meslala — matching the seeded <c>Varietes</c> rows)
/// each in three formats (1 L, 5 L, 10 L), so nine products laid out as one line per variety.
/// Prices are per-litre rates with a format multiplier, which is how the range is normally
/// built; they are demo figures, not quotations.
/// </para>
/// <para>
/// Photos come from the Chiadma shots in <c>wwwroot/images</c>. Within a variety the three
/// formats share a photo, since a bottle, a 5 L and a 10 L can of the same oil are not
/// separately photographed here; <see cref="BoutiqueVarieteViewModel.PhotoVariete"/> gives
/// each variety one image of its own for the landing page cards.
/// </para>
/// <para>
/// Replace with a service reading <c>Produits</c> joined to <c>Varietes</c> when the real
/// photography and stock are wired up. <see cref="BoutiqueProduitViewModel.ImageUrl"/> is
/// already nullable for that swap.
/// </para>
/// </summary>
public static class BoutiqueDemoCatalogue
{
    /// <summary>
    /// Formats offered for every variety, smallest first. The three Chiadma photos are
    /// assigned by position rather than by matching capacity, so all three varieties end up
    /// showing the same set. Recompute the photo order if the lineup changes.
    /// </summary>
    private static readonly (decimal Litres, string Emballage, decimal Coefficient, string Photo)[] Formats =
    [
        (1m, "bouteille", 1.00m, "~/images/chiadma 1l.jpeg"),
        (5m, "bidon", 0.84m, "~/images/chiadma 2l.jpeg"),
        (10m, "bidon", 0.76m, "~/images/chiadma 5l.jpeg"),
    ];

    // Each variety gets a different photoVariete so the landing page's three variety cards
    // don't all show the same bottle. Arbitrary assignment — the three Chiadma photos are all
    // the same region, not three varieties — but it is what makes the lineup legible.
    public static IReadOnlyList<BoutiqueVarieteViewModel> Varietes { get; } =
    [
        Build(1, "Picholine Marocaine", "PICH", "Fès-Meknès", 130m,
            "Pic et dru, equilibré. Amande douce en bouche, finals herbacés et une amertume "
            + "fine et tardive. La plus polyvalente de nos trois variétés.",
            Formats[0].Photo),
        Build(2, "Haouzia", "HAOU", "Marrakech-Safi", 150m,
            "Douceur ronde, presque sans piquant. Beurre froid et noisette, très peu "
            + "d'amertume. La plus facile à glisser dans un palais de tous les jours.",
            Formats[1].Photo),
        Build(3, "Meslala", "MESL", "Marrakech-Safi", 175m,
            "Forte, structurée, franche. Poivre vert à l'ouverture, tomate confite et "
            + "herbes sèches en finale. Celle qui tient un plat de résistance.",
            Formats[2].Photo),
    ];

    public static IReadOnlyList<BoutiqueProduitViewModel> Tous { get; } =
        Varietes.SelectMany(v => v.Produits).ToList();

    public static BoutiqueVarieteViewModel? TrouverVariete(int id)
        => Varietes.FirstOrDefault(v => v.Id == id);

    public static BoutiqueProduitViewModel? Trouver(int id)
        => Tous.FirstOrDefault(p => p.Id == id);

    private static BoutiqueVarieteViewModel Build(
        int varieteId,
        string nom,
        string code,
        string region,
        decimal prixParLitre,
        string description,
        string photoVariete)
    {
        var produits = new List<BoutiqueProduitViewModel>();
        var numero = 1;

        foreach (var (litres, emballage, coefficient, photo) in Formats)
        {
            produits.Add(new BoutiqueProduitViewModel
            {
                Id = varieteId * 100 + numero++,
                Reference = $"{code}-{litres:0}LT",
                Designation = $"Huile d'olive extra vierge {litres:0} L",
                VarieteId = varieteId,
                VarieteNom = nom,
                VarieteCode = code,
                VarieteRegion = region,
                Unite = emballage,
                ContenanceLitres = litres,
                ImageUrl = photo,
                PrixVenteHT = decimal.Round(prixParLitre * litres * coefficient, 2),
                TauxTVA = 20m,
                Disponible = true,
            });
        }

        return new BoutiqueVarieteViewModel
        {
            Id = varieteId,
            Nom = nom,
            Code = code,
            RegionOrigine = region,
            Description = description,
            PhotoVariete = photoVariete,
            Produits = produits,
        };
    }
}
