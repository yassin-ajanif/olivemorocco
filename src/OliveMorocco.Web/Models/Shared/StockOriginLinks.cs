using OliveMorocco.Business.Services.Stockage.Intrants;
using OliveMorocco.Business.Services.Stockage.Produits;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Models.Shared;

/// <summary>
/// Resolves the document a stock movement came from, so the ledger rows link back to it.
/// Returns <c>null</c> for origins that have no detail page (manual adjustment) or when the
/// document behind the movement has since been deleted.
/// </summary>
public static class StockOriginLinks
{
    public static string? ForProduit(string origineType, int? origineId) =>
        origineId is not int id
            ? null
            : origineType switch
            {
                StockProduitService.OrigineBonLivraison => $"/{AppSections.Vente}/BonsLivraison/Edit/{id}",
                StockProduitService.OrigineBonReception => $"/{AppSections.Achat}/BonsReception/Edit/{id}",
                StockProduitService.OrigineAvoirClient => $"/{AppSections.Vente}/Avoirs/Edit/{id}",
                StockProduitService.OrigineRemplissage => $"/{AppSections.Operationnel}/Remplissages/Edit/{id}",
                _ => null,
            };

    public static string? ForIntrant(string origineType, int? origineId) =>
        origineId is not int id
            ? null
            : origineType switch
            {
                StockIntrantService.OrigineBonReception => $"/{AppSections.Achat}/BonsReception/Edit/{id}",
                StockIntrantService.OrigineAvoirFournisseur => $"/{AppSections.Achat}/AvoirFournisseur/Edit/{id}",
                StockIntrantService.OrigineIntervention => $"/{AppSections.Operationnel}/Interventions/Edit/{id}",
                _ => null,
            };
}
