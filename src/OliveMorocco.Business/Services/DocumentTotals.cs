namespace OliveMorocco.Business.Services;

public static class DocumentTotals
{
    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);

    public static decimal PuTtc(decimal prixUnitaireHt, decimal tauxTva) =>
        RoundMoney(prixUnitaireHt * (1 + tauxTva / 100m));

    public static decimal LineTtc(decimal quantite, decimal prixUnitaireHt, decimal remise, decimal tauxTva) =>
        RoundMoney(Math.Max(0, RoundMoney(quantite * PuTtc(prixUnitaireHt, tauxTva)) - remise));

    public static decimal LineHt(decimal quantite, decimal prixUnitaireHt, decimal remise, decimal tauxTva)
    {
        var rate = 1 + tauxTva / 100m;
        return rate == 0 ? 0 : RoundMoney(LineTtc(quantite, prixUnitaireHt, remise, tauxTva) / rate);
    }

    public static (decimal Ht, decimal Tva, decimal Ttc) Compute(
        IEnumerable<(decimal Quantite, decimal PrixUnitaireHT, decimal Remise, decimal TauxTVA)> lignes,
        decimal remiseGlobale = 0)
    {
        decimal ht = 0, tva = 0;
        foreach (var (quantite, prixUnitaireHt, remise, tauxTva) in lignes)
        {
            var lineTtc = LineTtc(quantite, prixUnitaireHt, remise, tauxTva);
            var lineHt = LineHt(quantite, prixUnitaireHt, remise, tauxTva);
            ht += lineHt;
            tva += lineTtc - lineHt;
        }

        ht = Math.Max(0, ht - remiseGlobale);
        return (ht, tva, ht + tva);
    }
}
