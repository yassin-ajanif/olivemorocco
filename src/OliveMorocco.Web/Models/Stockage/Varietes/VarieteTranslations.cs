namespace OliveMorocco.Web.Models.Stockage.Varietes;

/// <summary>
/// Arabic for the Varieties document.
///
/// A variety is the agricultural fact — Picholine, Beldi — that products, pressings and
/// bulk oil are all measured against. Where <c>Stock huile (L)</c> appears here it is
/// variety oil stock, which is a different number from a product's stock and is deliberately
/// labelled differently rather than sharing one word.
/// </summary>
public static class VarieteTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Variétés"] = "الأصناف",
            ["variété"] = "صنف",
            ["Modifier variété"] = "تعديل الصنف",
            ["Nouvelle variété"] = "صنف جديد",

            // --- Identity ---
            ["Nom"] = "الاسم",
            ["Code"] = "الرمز",
            ["Région d'origine"] = "منطقة الأصل",
            ["Région"] = "المنطقة",

            // --- Stock of this variety's oil, before bottling ---
            ["Stock huile (L)"] = "مخزون الزيت (لتر)",

            // --- Lists ---
            ["Picholine, Beldi et autres variétés d'olive"] = "بشولين وبلدي وأصناف زيتون أخرى",
            ["Rechercher par nom, code ou région…"] = "البحث بالاسم أو الرمز أو المنطقة…",

            ["Supprimer cette variété ?"] = "حذف هذا الصنف؟",

            // --- The breadcrumb above this document's forms: its section, then itself ---
            ["Stockage · Variétés"] = "التخزين · الأصناف",
        };
}
