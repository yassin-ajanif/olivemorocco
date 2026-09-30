using OliveMorocco.Web.Models.Shared;

namespace OliveMorocco.Web.Models.Vente.BonsCommande;

/// <summary>
/// Arabic for the customer order document.
///
/// Named apart from <c>Achat\BonsCommande\BonCommandeAchatTranslations</c> even though the
/// two are the same shape, because a customer order and a supplier order are different
/// documents: one sells, one buys, and the Arabic for the singular form differs by the same
/// word. Their shared line table comes from <see cref="SalesDocumentTranslations"/>.
/// </summary>
public static class BonCommandeVenteTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {

            ["Commandes clients"] = "طلبات الزبائن",

            ["Client"] = "الزبون",

        };
}
