using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class FournisseursController : DashboardModuleController
{
    [HttpGet("")]
    public IActionResult Index() => ModuleList(new ModuleListPageViewModel
    {
        Section = "Achat",
        ModuleKey = "fournisseurs",
        Title = "Fournisseurs",
        Subtitle = "Répertoire fournisseurs",
        Headers = ["Nom", "ICE", "Ville", "Actif"],
        GridColumns = "2fr 1fr 1fr auto",
    });

    [HttpGet("Create")]
    public IActionResult Create() => RedirectToAction(nameof(Index));
}
