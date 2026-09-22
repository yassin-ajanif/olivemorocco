using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/[controller]")]
public sealed class DevisController : DashboardModuleController
{
    [HttpGet("")]
    public IActionResult Index() => ModuleList(new ModuleListPageViewModel
    {
        Section = "Vente",
        ModuleKey = "devis",
        Title = "Devis",
        Subtitle = "Devis commerciaux",
        Headers = ["Réf.", "Client", "Date", "Validité", "TTC", "Note"],
        GridColumns = "140px 1fr 100px 110px 100px 1fr",
    });

    [HttpGet("Create")]
    public IActionResult Create() => RedirectToAction(nameof(Index));
}
