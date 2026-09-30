namespace OliveMorocco.Web.Models.Vente.Avoirs;

/// <summary>
/// Arabic for the customer credit note.
///
/// An avoir cancels part of an invoice, so it is the only sales document that points
/// backwards at another document by number and asks for a reason. Those two fields are the
/// whole of what is specific to it.
/// </summary>
public static class AvoirVenteTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Avoirs"] = "الإرجاعات",
            ["avoir"] = "إرجاع",
            ["Avoirs clients"] = "إرجاعات الزبائن",
            ["Nouvel avoir"] = "إرجاع جديد",
            ["Modifier avoir"] = "تعديل الإرجاع",

            // --- Why, and which invoice it cancels ---
            ["Motif"] = "السبب",
            ["N° facture (ID)"] = "رقم الفاتورة (المعرّف)",

            ["Client"] = "الزبون",

            ["Supprimer cet avoir ?"] = "حذف هذا الإرجاع؟",
        };
}
