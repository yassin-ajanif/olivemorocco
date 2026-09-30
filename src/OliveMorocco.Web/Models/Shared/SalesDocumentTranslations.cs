namespace OliveMorocco.Web.Models.Shared;

/// <summary>
/// The vocabulary every sales or purchase document shares: the line-item table, the
/// totals block, the buttons that act on a row, and the payments block under the totals.
///
/// <para>
/// This is the one place where a second shared file is genuinely necessary rather than
/// convenient. Nine documents — devis, commande, livraison, facture, avoir on the sales
/// side, and their four purchase counterparts — print the same thirteen column headings
/// and the same totals labels, because they are all the same table with a different
/// counterparty. Spread across nine per-document files that is over a hundred entries to
/// keep in step, and the day a column was renamed in one form and not the other eight the
/// dashboard would be calling the same column two different things in two languages at once.
///
/// <see cref="UiTranslations"/> holds the shell's own words (buttons, pagination, section
/// names) because they belong to no document. This file holds words that belong to the
/// <em>document</em> shape — the part any invoice shares with any other invoice — rather
/// than to any one of them.
/// </para>
///
/// <para>
/// What stays per-document is the counterparty and the document's own particulars: a devis
/// has a validity date, a reception has a received quantity, a credit note has a reason.
/// Those go in the document's own file, where a reader can see the whole vocabulary of
/// that document in one place.
/// </para>
/// </summary>
public static class SalesDocumentTranslations
{
    public static IReadOnlyDictionary<string, string> Arabic { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            // --- The line-item table. Same thirteen columns in every document. ---
            ["Réf."] = "المرجع",
            ["Désignation"] = "التسمية",
            ["Qté"] = "الكمية",
            ["Unité"] = "الوحدة",
            ["PU HT"] = "سعر الوحدة دون ضريبة",
            ["PU TTC"] = "سعر الوحدة مع الضريبة",
            ["Remise"] = "التخفيض",
            ["TVA %"] = "نسبة الضريبة %",
            ["Montant TTC"] = "المبلغ مع الضريبة",

            // --- The totals block ---
            ["Totaux"] = "المجاميع",
            ["Total HT"] = "المجموع دون ضريبة",
            ["TVA"] = "الضريبة",
            ["Total TTC"] = "المجموع مع الضريبة",

            // --- Acting on a line ---
            ["Ajouter un article"] = "إضافة منتج",
            ["Ajouter un intrant"] = "إضافة مدخل",
            ["Supprimer la ligne"] = "حذف السطر",
            ["Sélectionner la ligne"] = "تحديد السطر",
            ["Afficher ou masquer le détail de la ligne"] = "إظهار أو إخفاء تفاصيل السطر",
            ["Sélection"] = "اختيار",

            // --- Search boxes, one per counterparty kind. The wording differs only in the
            //     word for the counterparty, which is why these are keyed whole rather than
            //     composed at runtime: a translator should be able to reword the sentence. ---
            ["Rechercher un client…"] = "البحث عن زبون…",
            ["Rechercher un fournisseur…"] = "البحث عن مورد…",
            ["Rechercher un intrant par nom…"] = "البحث عن مدخل بالاسم…",
            ["Rechercher par référence ou désignation…"] = "البحث بالمرجع أو التسمية…",
            ["Rechercher par n° ou client…"] = "البحث برقم أو زبون…",
            ["Rechercher par n° ou fournisseur…"] = "البحث برقم أو مورد…",

            // --- Document header fields present on most of them ---
            ["Référence"] = "المرجع",
            ["Date"] = "التاريخ",
            ["N°"] = "رقم",
            ["Échéance"] = "تاريخ الاستحقاق",
            ["Payée"] = "مدفوعة",
            ["Non payée"] = "غير مدفوعة",
            ["Statut"] = "الحالة",
            ["Vers facture"] = "إلى الفاتورة",
            ["Facture"] = "الفاتورة",
            ["Réf. bon de commande"] = "مرجع أمر الطلب",

            // --- The documents that exist in both directions. A customer order and a
            //     supplier order are the same document pointing opposite ways, so they are
            //     named by the same words; what differs is the counterparty, and that is a
            //     field label further down. If one side ever needs different wording its own
            //     file overrides this, because a document merges after what it shares. ---
            ["Bons de commande"] = "أوامر الطلب",
            ["bon de commande"] = "أمر الطلب",
            ["Nouveau bon de commande"] = "أمر طلب جديد",
            ["Modifier bon de commande"] = "تعديل أمر الطلب",
            ["Supprimer ce bon de commande ?"] = "حذف أمر الطلب هذا؟",
            ["Bons de livraison"] = "أوامر التسليم",
            ["Bons de réception"] = "أوامر الاستلام",

            // Both sides settle the same two ways, so neither has its own wording for them.
            ["Payé (encaissé)"] = "مدفوع (محصَّل)",
            ["Reste dû"] = "المبلغ المتبقي",
            ["Supprimer cette facture ?"] = "حذف هذه الفاتورة؟",

            // --- "Nothing found" in a suggestion box ---
            ["Aucun article trouvé."] = "لم يتم العثور على أي منتج.",
            ["Aucun client trouvé."] = "لم يتم العثور على أي زبون.",
            ["Aucun fournisseur trouvé."] = "لم يتم العثور على أي مورد.",
            ["Aucun intrant trouvé."] = "لم يتم العثور على أي مدخل.",

            // --- The hint under an empty line table. Two of them because one document
            //     adds stock and the other adds inputs. ---
            ["Ajoutez des articles via la recherche ci-dessus."] =
                "أضف منتجات عبر البحث أعلاه.",
            ["Ajoutez des intrants via la recherche ci-dessus."] =
                "أضف مدخلات عبر البحث أعلاه.",

            // --- Payments, the block an invoice carries under its totals. Cash actually
            //     collected is not the same as invoiced, which is why the word here is
            //     "encaissé" — collected — rather than "payé" — paid. ---
            ["Paiements"] = "الدفعات",
            ["Ajouter un paiement"] = "إضافة دفعة",
            ["Montant"] = "المبلغ",
            ["Mode"] = "الطريقة",
            ["Encaissé"] = "مُحصَّل",
            ["Supprimer le paiement"] = "حذف الدفعة",
            ["N° chèque, virement…"] = "رقم الشيك، تحويل…",
            ["Total encaissé :"] = "المحصَّل إجمالاً:",
            ["En attente :"] = "في الانتظار:",
            ["Aucun paiement — ajoutez une ligne pour enregistrer un règlement."] =
                "لا توجد دفعات — أضف سطرًا لتسجيل تسوية.",
        };
}