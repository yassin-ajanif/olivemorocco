namespace OliveMorocco.Web.Models.Achat.Charges;

/// <summary>
/// Arabic for the Charges document: what the estate spends beyond buying inputs.
///
/// A charge is either typed on its own or raised from an intervention, which is why its
/// list carries wording the other documents do not — that a charge reached from an
/// intervention cannot be edited from the list, and where to go to do it instead.
/// </summary>
public static class ChargeTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Charges"] = "المصاريف",
            ["charge"] = "مصروف",
            ["Nouvelle charge"] = "مصروف جديد",
            ["Modifier charge"] = "تعديل المصروف",
            ["Frais liés aux achats et à l'exploitation"] =
                "مصاريف مرتبطة بالمشتريات والاستغلال",

            // --- A charge is a kind, a description, a date and an amount ---
            ["Type"] = "النوع",
            ["Libellé"] = "البيان",

            ["Note"] = "ملاحظة",

            // --- When it came from an intervention, it is edited there ---
            ["Intervention"] = "التدخل",
            ["Modifier depuis l'intervention"] = "تعديل من التدخل",
            ["Modifiable depuis l'intervention"] = "يُعدَّل من التدخل",
            ["Modifier (depuis l'intervention)"] = "تعديل (من التدخل)",

            // --- Lists ---
            ["Rechercher par libellé ou type…"] = "البحث بالبيان أو النوع…",

            ["Supprimer cette charge ?"] = "حذف هذا المصروف؟",

            // --- The breadcrumb above this document's forms: its section, then itself ---
            ["Achat · Charges"] = "الشراء · المصاريف",
        };
}
