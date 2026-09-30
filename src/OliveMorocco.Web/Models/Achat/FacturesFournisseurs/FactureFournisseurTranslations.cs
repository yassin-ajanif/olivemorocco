namespace OliveMorocco.Web.Models.Achat.FacturesFournisseurs;

/// <summary>
/// Arabic for the supplier invoice.
///
/// The buying mirror of the customer invoice, and like it the only purchase document that
/// tracks money owed. It also records which receptions it is paying for, so it carries the
/// receipt reference where a customer invoice carries the delivery-note one.
/// </summary>
public static class FactureFournisseurTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Factures fournisseur"] = "فواتير الموردين",
            ["facture fournisseur"] = "فاتورة المورد",
            ["Factures fournisseurs"] = "فواتير الموردين",
            ["Nouvelle facture fournisseur"] = "فاتورة مورد جديدة",
            ["Modifier facture fournisseur"] = "تعديل فاتورة المورد",

            // --- Money ---

            // --- What it is paying for ---

            ["Fournisseur"] = "المورد",

        };
}
