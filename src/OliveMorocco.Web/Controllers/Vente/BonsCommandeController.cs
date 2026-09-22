using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/BonsCommande")]
public sealed class BonsCommandeController : DashboardModuleController
{
    [HttpGet("")]
    public IActionResult Index() => ModuleList(new ModuleListPageViewModel
    {
        Section = "Vente",
        ModuleKey = "bons-commande",
        Title = "Bons de commande",
        Subtitle = "Commandes clients",
        Headers = ["Réf.", "Client", "Date", "TTC", "Note"],
        GridColumns = "120px 1fr 100px 100px 1fr",
    });

    [HttpGet("Create")]
    public IActionResult Create() => RedirectToAction(nameof(Index));
}
