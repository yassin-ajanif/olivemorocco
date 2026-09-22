using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/[controller]")]
public sealed class FacturationController : DashboardModuleController
{
    [HttpGet("")]
    public IActionResult Index() => ModuleList(new ModuleListPageViewModel
    {
        Section = "Vente",
        ModuleKey = "facturation",
        Title = "Facturation",
        Subtitle = "Factures clients",
        Headers = ["Réf.", "Client", "Date", "Échéance", "Payée", "TTC", "Note"],
        GridColumns = "1fr 1fr 100px 100px 90px 100px 1fr",
    });

    [HttpGet("Create")]
    public IActionResult Create() => RedirectToAction(nameof(Index));
}
