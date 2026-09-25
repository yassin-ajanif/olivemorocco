namespace OliveMorocco.Web.Routing;

/// <summary>Sidebar + page chrome derived from the current MVC route (no ViewData in controllers).</summary>
public static class DashboardNav
{
    public sealed record Module(string Section, string ModuleKey, string Label, string SingularLabel);

    private static readonly Dictionary<string, Module> ByController = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Produits"] = new("stockage", "produits", "Produits & variétés", "produit"),
        ["Intrants"] = new("stockage", "intrants", "Intrants", "intrant"),
        ["Secteurs"] = new("stockage", "secteurs", "Secteurs", "secteur"),
        ["Stock"] = new("stockage", "stock", "État du stock", "stock"),
        ["Clients"] = new("vente", "clients", "Clients", "client"),
        ["Devis"] = new("vente", "devis", "Devis", "devis"),
        ["BonsCommande"] = new("vente", "bons-commande", "Bons de commande", "bon de commande"),
        ["BonsLivraison"] = new("vente", "bons-livraison", "Bons de livraison", "bon de livraison"),
        ["Facturation"] = new("vente", "facturation", "Facturation", "facture"),
        ["Avoirs"] = new("vente", "avoirs", "Avoirs", "avoir"),
        ["Fournisseurs"] = new("achat", "fournisseurs", "Fournisseurs", "fournisseur"),
        ["BonsCommandeAchat"] = new("achat", "bons-commande-achat", "Bons de commande", "bon de commande"),
        ["BonsReception"] = new("achat", "bons-reception", "Bons de réception", "bon de réception"),
        ["FacturesFournisseurs"] = new("achat", "factures-fournisseurs", "Factures fournisseur", "facture fournisseur"),
        ["AvoirFournisseur"] = new("achat", "avoir-fournisseur", "Avoir fournisseur", "avoir fournisseur"),
        ["Charges"] = new("achat", "charges", "Charges", "charge"),
        ["Interventions"] = new("operationnel", "interventions", "Interventions", "intervention"),
    };

    public static Module? Resolve(HttpContext http)
    {
        var controller = http.GetRouteValue("controller")?.ToString();
        if (controller is not null && ByController.TryGetValue(controller, out var module))
            return module;

        return ResolveFromPath(http.Request.Path.Value);
    }

    public static string PageTitle(HttpContext http, Module? module)
    {
        if (module is null)
            return "Dashboard";

        var action = http.GetRouteValue("action")?.ToString();
        var path = http.Request.Path.Value ?? "";

        if (action is "Create" || path.EndsWith("/Create", StringComparison.OrdinalIgnoreCase))
            return $"Nouveau {module.SingularLabel}";

        if (action is "Edit" || path.Contains("/Edit/", StringComparison.OrdinalIgnoreCase))
            return $"Modifier {module.SingularLabel}";

        return module.Label;
    }

    private static Module? ResolveFromPath(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        foreach (var (controller, module) in ByController)
        {
            if (path.Contains($"/{AppSections.Stockage}/{controller}", StringComparison.OrdinalIgnoreCase)
                || path.Contains($"/{AppSections.Vente}/{controller}", StringComparison.OrdinalIgnoreCase)
                || path.Contains($"/{AppSections.Achat}/{controller}", StringComparison.OrdinalIgnoreCase)
                || path.Contains($"/{AppSections.Operationnel}/{controller}", StringComparison.OrdinalIgnoreCase))
            {
                return module;
            }
        }

        return null;
    }
}
