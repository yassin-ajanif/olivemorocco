using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/BonsCommande")]
public sealed class BonsCommandeAchatController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("ModuleComingSoon");
}
