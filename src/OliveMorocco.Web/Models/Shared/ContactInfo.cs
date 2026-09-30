namespace OliveMorocco.Web.Models.Shared;

/// <summary>
/// The public site's contact details, in one place.
///
/// Previously the number was typed into the footer, the landing page CTA and the product
/// page separately, and they drifted apart — a second number appeared for the WhatsApp
/// button while three other places still showed the old one. Every link and every visible
/// number now comes from here, so there is one edit to make when it changes.
/// </summary>
public static class ContactInfo
{
    /// <summary>
    /// Country code and national number, digits only — the form both <c>tel:</c> and
    /// <c>wa.me</c> want. Written without punctuation so it cannot be pasted into a URL
    /// with spaces or dashes in it.
    /// </summary>
    public const string Digits = "212611625441";

    /// <summary>
    /// The same number as it is printed on the page. Keep in step with <see cref="Digits"/>:
    /// it is a separate literal only because the printed form has spaces in it.
    /// </summary>
    public const string Display = "+212 611 62 54 41";

    /// <summary>
    /// What the floating WhatsApp button pre-fills. A visitor who has to invent the
    /// opening line is a visitor who closes the tab instead.
    /// </summary>
    private const string WhatsAppDefaultMessage =
        "Bonjour OliveMorocco, je souhaite des informations sur vos huiles d'olive.";

    /// <summary>URI for the "call us" links.</summary>
    public static string TelUri => $"tel:+{Digits}";

    /// <summary>
    /// A <c>wa.me</c> link that opens WhatsApp with a message already typed, which the
    /// visitor can edit or clear. Pass <paramref name="message"/> to mention what the page
    /// is about — a product page asks about that bottle rather than in general.
    /// </summary>
    public static string WhatsApp(string? message = null) =>
        $"https://wa.me/{Digits}?text={Uri.EscapeDataString(message ?? WhatsAppDefaultMessage)}";
}