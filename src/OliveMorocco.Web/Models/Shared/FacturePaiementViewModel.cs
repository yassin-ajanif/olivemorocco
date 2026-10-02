using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Web.Models.Shared;

public sealed class FacturePaiementViewModel
{
    public DateTime Date { get; set; } = DateTime.Today;

    public decimal Montant { get; set; }

    public ModePaiement Mode { get; set; } = ModePaiement.Virement;

    public string? Reference { get; set; }

    /// <summary>
    /// Nullable on purpose. An unchecked checkbox posts nothing at all, so this has to be
    /// able to say "absent" — a plain bool cannot tell an unticked box from a missing
    /// field, which is why this used to be paired with a hidden false input sharing the same
    /// name. The binder takes the first value posted under a name, so that hidden input won
    /// every time and the checkbox was silently discarded.
    /// The view posts no hidden companion; a ticked box sends "true" and an unticked one
    /// sends nothing, and the controllers coalesce with ?? false.
    /// </summary>
    public bool? EstEncaisse { get; set; } = true;
}
