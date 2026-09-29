namespace OliveMorocco.Business.Services;

/// <summary>
/// Stock effect of an avoir's lines, used to work out the correction an edit has to apply
/// to the movement ledger. Both avoir flows share the shape but not the sign: a client
/// credit note brings goods back in, a supplier credit note sends them out.
/// </summary>
internal static class AvoirStockEffect
{
    /// <summary>
    /// Per-item signed quantity, summed when the same item appears on several lines.
    /// Returns an empty map when <c>retourMarchandise</c> is off — a purely financial
    /// credit note must not touch stock.
    /// </summary>
    /// <param name="signe">+1 when a goods return adds stock, -1 when it removes stock.</param>
    public static Dictionary<int, decimal> Build(
        bool retourMarchandise,
        IEnumerable<(int ItemId, decimal Quantite)> lignes,
        int signe)
    {
        var effect = new Dictionary<int, decimal>();

        if (!retourMarchandise)
            return effect;

        foreach (var (itemId, quantite) in lignes)
            effect[itemId] = effect.GetValueOrDefault(itemId) + signe * quantite;

        return effect;
    }

    /// <summary>
    /// Difference between what the lines used to do to stock and what they do now, as a
    /// single signed delta per item. Covers a quantity edit and a change of the
    /// <c>retourMarchandise</c> flag with the same call.
    /// </summary>
    public static Dictionary<int, decimal> Delta(
        bool ancienRetourMarchandise,
        IEnumerable<(int ItemId, decimal Quantite)> lignesAvant,
        bool nouveauRetourMarchandise,
        IEnumerable<(int ItemId, decimal Quantite)> lignesApres,
        int signe)
    {
        var avant = Build(ancienRetourMarchandise, lignesAvant, signe);
        var apres = Build(nouveauRetourMarchandise, lignesApres, signe);

        // The ledger already reflects `avant`, so the correction is `apres - avant`:
        // start from the negated old effect, then add the new one.
        var delta = new Dictionary<int, decimal>();
        foreach (var (itemId, quantite) in avant)
            delta[itemId] = -quantite;

        foreach (var (itemId, quantite) in apres)
            delta[itemId] = delta.GetValueOrDefault(itemId) + quantite;

        return delta;
    }
}
