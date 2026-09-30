using OliveMorocco.Web.Models.Shared;

namespace OliveMorocco.Web.Models.Achat.BonsCommande;

/// <summary>
/// Arabic for the supplier order.
///
/// The buying mirror of the customer order. The sidebar labels both documents
/// "Bons de commande" and both mean the same thing, so that key resolves identically from
/// both files; what differs is the counterparty and therefore the search box and the
/// singular form.
/// </summary>
public static class BonCommandeAchatTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {

            ["Commandes fournisseurs"] = "طلبات الموردين",

            ["Fournisseur"] = "المورد",

        };
}
