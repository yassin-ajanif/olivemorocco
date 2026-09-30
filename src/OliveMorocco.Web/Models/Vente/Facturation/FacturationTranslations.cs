namespace OliveMorocco.Web.Models.Vente.Facturation;

/// <summary>
/// Arabic for the customer invoice.
///
/// The only sales document that talks about money owed rather than goods sent: it has a
/// due date, a settled/unsettled state, and an amount still outstanding. Those four labels
/// are its own vocabulary; the rest of it is the shared document shape.
/// </summary>
public static class FacturationTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Facturation"] = "الفوترة",
            ["facture"] = "فاتورة",
            ["Factures clients"] = "فواتير الزبائن",
            ["Nouvelle facture"] = "فاتورة جديدة",
            ["Modifier facture"] = "تعديل الفاتورة",

            // --- Money, which is what distinguishes this document ---

            // --- What it was raised from ---

            ["Client"] = "الزبون",

        };
}
