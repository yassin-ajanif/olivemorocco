namespace OliveMorocco.Web.Models.Stockage.Secteurs;

/// <summary>
/// Arabic for the Plots document: the sectors of the estate and how their area is split
/// between varieties.
///
/// Area is the reason this document exists — a sector's hectares are divided across
/// varieties, and an intervention is booked against a sector. So "Superficie" carries its
/// unit in the label ("ha") rather than leaving the reader to infer it.
/// </summary>
public static class SecteurTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Secteurs"] = "القطاعات",
            ["secteur"] = "قطاع",
            ["Nouveau secteur"] = "قطاع جديد",
            ["Modifier secteur"] = "تعديل القطاع",
            ["Parcelles et secteurs de l'exploitation"] = "المزاطق والقطاعات المستغلّة",

            // --- Identity ---
            ["Nom"] = "الاسم",
            ["Code"] = "الرمز",
            ["SN, PS…"] = "SN, PS…",

            // --- Area, and how it is split ---
            ["Superficie"] = "المساحة",
            ["Superficie totale (ha)"] = "المساحة الإجمالية (هكتار)",
            ["Superficie (ha)"] = "المساحة (هكتار)",
            ["Répartition par variété"] = "التوزيع حسب الصنف",
            ["Variété"] = "الصنف",
            ["Variétés"] = "الأصناف",
            ["Gérer les variétés"] = "إدارة الأصناف",
            ["+ Ajouter une ligne"] = "+ إضافة سطر",

            // --- Lists ---
            ["Rechercher par nom ou code…"] = "البحث بالاسم أو الرمز…",

            ["Supprimer ce secteur ?"] = "حذف هذا القطاع؟",

            // --- The breadcrumb above this document's forms: its section, then itself ---
            ["Stockage · Secteurs"] = "التخزين · القطاعات",
        };
}
