using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class BonsReceptionController : DashboardModuleController
{
    [HttpGet("")]
    public IActionResult Index() => ModuleList(new ModuleListPageViewModel
    {
        Section = "Achat",
        ModuleKey = "bons-reception",
        Title = "Bons de réception",
        Subtitle = "Réceptions fournisseurs",
        Headers = ["Réf.", "Fournisseur", "Date", "TTC", "Note"],
        GridColumns = "120px 1fr 100px 100px 1fr",
    });

    [HttpGet("Create")]
    public IActionResult Create() => RedirectToAction(nameof(Index));
}
