namespace OliveMorocco.Web.Models.Stockage.Intrants;

/// <summary>
/// Arabic for the Inputs document: fertiliser, compost and the other agricultural inputs
/// consumed by an intervention.
///
/// An intrant is bought (a purchase order against a supplier), received (a reception) and
/// then consumed (an intervention). Those three documents each say "intrants" and all three
/// resolve here, so an input cannot be called one thing on the way in and another on the way
/// out.
/// </summary>
public static class IntrantTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Intrants"] = "المدخلات",
            ["intrant"] = "مدخل",
            ["Nouvel intrant"] = "مدخل جديد",
            ["Modifier intrant"] = "تعديل المدخل",
            ["Engrais, compost et autres intrants agricoles"] = "أسمدة وسماد ومدخلات فلاحية أخرى",

            // --- Identity and pricing ---
            ["Nom"] = "الاسم",

            ["PU HT (DH)"] = "سعر الوحدة دون ضريبة (درهم)",
            ["kg, L, tonne, sac 25 kg…"] = "كغ، لتر، طن، كيس 25 كغ…",

            // --- Stock ---
            ["Stock"] = "المخزون",

            // --- Lists ---
            ["Rechercher par nom…"] = "البحث بالاسم…",

            ["Supprimer cet intrant ?"] = "حذف هذا المدخل؟",

            // --- The breadcrumb above this document's forms: its section, then itself ---
            ["Stockage · Intrants"] = "التخزين · المدخلات",
        };
}
