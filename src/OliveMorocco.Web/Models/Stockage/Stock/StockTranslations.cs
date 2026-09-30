namespace OliveMorocco.Web.Models.Stockage.Stock;

/// <summary>
/// Arabic for the Stock document and the three adjustment pages it opens.
///
/// Three different stocks live behind this one route — bottled products, bulk oil by
/// variety, and inputs — so the module has to say which it means. The pages therefore name
/// their stock in the eyebrow ("Stockage · Huile en vrac"), which is why the three
/// variations are keyed separately rather than sharing one word for "stock".
///
/// The movement history is the same shape on all three pages, so that vocabulary is here
/// once: what a movement is, where it came from, and the before/after pair.
/// </summary>
public static class StockTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            // --- The list and its three tabs ---
            ["État du stock"] = "حالة المخزون",
            ["stock"] = "مخزون",
            ["Produits"] = "المنتجات",
            ["Huile en vrac (variétés)"] = "الزيت بالجملة (الأصناف)",
            ["Intrants"] = "المدخلات",
            ["Stock bas uniquement"] = "المخزون المنخفض فقط",

            // --- The list's one-line description, which changes per tab ---
            ["Quantités disponibles et alertes de réapprovisionnement"] =
                "الكميات المتاحة وتنبيهات إعادة التموين",
            ["Huile issue des pressages, stockée par variété avant mise en bouteille"] =
                "الزيت الناتج عن العصر، مخزّن حسب الصنف قبل التعبئة",
            ["Intrants disponibles pour les interventions"] =
                "المدخلات المتاحة للتدخلات",

            // --- The three detail pages' eyebrows ---
            ["Stockage · Huile en vrac"] = "التخزين · زيت بالجملة",
            ["Stockage · Stock des intrants"] = "التخزين · مخزون المدخلات",
            ["Stockage · État du stock"] = "التخزون · حالة المخزون",

            // --- Columns, per stock kind ---
            ["Variété"] = "الصنف",
            ["Code"] = "الرمز",
            ["Stock (L)"] = "المخزون (لتر)",

            ["Nom"] = "الاسم",

            ["Stock actuel"] = "المخزون الحالي",
            ["Prix achat HT"] = "سعر الشراء دون ضريبة",

            ["Stock min."] = "الحد الأدنى للمخزون",
            ["Type de stock"] = "نوع المخزون",

            // --- Status values ---
            ["Vide"] = "نفد",
            ["Disponible"] = "متوفر",
            ["OK"] = "مقبول",
            ["Stock bas"] = "مخزون منخفض",
            ["Rupture"] = "نفاد",

            // --- Manual adjustment ---
            ["Ajustement manuel"] = "تعديل يدوي",
            ["Variation"] = "التغيّر",
            ["Variation (L)"] = "التغيّر (لتر)",
            ["Enregistrer l'ajustement"] = "حفظ التعديل",
            ["Ex. 50 ou -12"] = "مثال 50 أو ‎-12",
            ["Ex. 10 ou -5"] = "مثال 10 أو ‎-5",
            ["Motif de l'ajustement (optionnel)"] = "سبب التعديل (اختياري)",
            ["Note"] = "ملاحظة",

            ["Entrez une variation en litres : positive (entrée) ou négative (sortie)."] =
                "أدخل تغيّراً باللتر: موجبًا (إدخال) أو سالبًا (إخراج).",
            ["Entrez une variation positive (entrée) ou négative (sortie)."] =
                "أدخل تغيّرًا موجبًا (إدخال) أو سالبًا (إخراج).",

            // --- Movement history, identical on all three pages ---
            ["Historique des mouvements"] = "سجل الحركات",
            ["Historique"] = "السجل",
            ["Type"] = "النوع",
            ["Stock avant"] = "المخزون قبل",
            ["Stock avant (L)"] = "المخزون قبل (لتر)",
            ["Quantité"] = "الكمية",
            ["Quantité (L)"] = "الكمية (لتر)",
            ["Stock après"] = "المخزون بعد",
            ["Stock après (L)"] = "المخزون بعد (لتر)",
            ["Origine"] = "المصدر",
            ["Entrée"] = "إدخال",
            ["Sortie"] = "إخراج",
            ["Ajustement"] = "تعديل",

            ["← Retour à la liste"] = "رجوع إلى القائمة",
        };
}
