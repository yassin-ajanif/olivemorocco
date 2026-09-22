using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class AvoirFournisseurController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("ModuleComingSoon");
}
