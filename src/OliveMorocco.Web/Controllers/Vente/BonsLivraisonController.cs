using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/[controller]")]
public sealed class BonsLivraisonController : DashboardModuleController
{
    [HttpGet("")]
    public IActionResult Index() => ModuleList(new ModuleListPageViewModel
    {
        Section = "Vente",
        ModuleKey = "bons-livraison",
        Title = "Bons de livraison",
        Subtitle = "Expéditions clients",
        Headers = ["Réf.", "Client", "Date", "TTC", "Note", "Statut"],
        GridColumns = "120px 1fr 100px 100px 1fr 120px",
    });

    [HttpGet("Create")]
    public IActionResult Create() => RedirectToAction(nameof(Index));
}
