using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/[controller]")]
public sealed class AvoirsController : DashboardModuleController
{
    [HttpGet("")]
    public IActionResult Index() => ModuleList(new ModuleListPageViewModel
    {
        Section = "Vente",
        ModuleKey = "avoirs",
        Title = "Avoirs",
        Subtitle = "Notes de crédit clients",
        Headers = ["Réf.", "Client", "Date", "TTC", "Note"],
        GridColumns = "120px 1fr 100px 100px 1fr",
    });

    [HttpGet("Create")]
    public IActionResult Create() => RedirectToAction(nameof(Index));
}
