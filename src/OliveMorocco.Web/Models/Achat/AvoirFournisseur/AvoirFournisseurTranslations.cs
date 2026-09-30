namespace OliveMorocco.Web.Models.Achat.AvoirFournisseur;

/// <summary>
/// Arabic for the supplier credit note.
///
/// The buying mirror of the customer credit note: it cancels part of a supplier invoice, so
/// it asks for a reason and points at the invoice by number. Its lines are inputs rather
/// than products, which is the one place the wording differs from the sales-side avoir.
/// </summary>
public static class AvoirFournisseurTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Avoir fournisseur"] = "إرجاع المورد",
            ["avoir fournisseur"] = "إرجاع المورد",
            ["Avoirs fournisseurs"] = "إرجاعات الموردين",
            ["Nouvel avoir fournisseur"] = "إرجاع مورد جديد",
            ["Modifier avoir fournisseur"] = "تعديل إرجاع المورد",

            // --- Why, and which invoice it cancels ---
            ["Motif"] = "السبب",

            ["Fournisseur"] = "المورد",

            ["Supprimer cet avoir ?"] = "حذف هذا الإرجاع؟",
        };
}
