using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class ChargesController : DashboardModuleController
{
    [HttpGet("")]
    public IActionResult Index() => ModuleList(new ModuleListPageViewModel
    {
        Section = "Achat",
        ModuleKey = "charges",
        Title = "Charges",
        Subtitle = "Charges d'exploitation",
        Headers = ["Type", "Date", "Libellé", "Bénéficiaire", "TTC", "Note"],
        GridColumns = "110px 100px 1fr 140px 100px 1fr",
    });

    [HttpGet("Create")]
    public IActionResult Create() => RedirectToAction(nameof(Index));
}
