namespace OliveMorocco.Web.Models.Shared;

/// <summary>
/// Marks a static class as a document's Arabic label table, so <see cref="Translations"/>
/// can find it and merge it in.
///
/// <para>
/// An attribute rather than an interface because a translation table is a bag of constants
/// with no behaviour, and C# does not allow a <c>static</c> class to implement an interface
/// (CS0714) even when the interface member is <c>static abstract</c>. Making the classes
/// non-static just to satisfy an interface would invent a constructor and an instance for
/// something that has neither. An attribute also states the intent where the table is
/// declared, instead of relying on a shape the merge happens to recognise.
/// </para>
///
/// <para>
/// Each document that shows text to the user owns one of these — Clients has
/// Models/Vente/Clients/ClientTranslations.cs, Produits will have
/// Models/Stockage/Produits/ProductTranslations.cs, and so on. Adding a document is dropping
/// in a file. There is no central list that each new module has to remember to edit, which is
/// the mistake one shared dictionary invites: twenty documents all reaching into one pile of
/// words, and nobody able to say which half of it belongs to Clients.
/// </para>
///
/// <para>
/// The table is keyed by the exact French string the view prints, not by an identifier
/// invented alongside it. That is deliberate: it makes it impossible for the sidebar, a page
/// heading and a browser tab title to disagree about what "Devis" is called, because all three
/// ask for the same key and therefore get the same answer. Keys are case-sensitive and must
/// match the markup byte for byte; <see cref="Translations.For"/> returns <c>null</c> for
/// anything it does not know, so a label can be added to a page before its Arabic is written
/// and it simply renders without a gloss.
/// </para>
///
/// <para>
/// One word, one meaning, one place. If a French label means the same thing in every document
/// it is not any document's — <see cref="UiTranslations"/> holds those. If two tables claim the
/// same word with different Arabic, the merge is deterministic but it cannot tell you which
/// was meant, so treat it as a bug rather than a tie to break.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TranslationsAttribute : Attribute;
