using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Achat;

[Route(AppSections.Achat + "/[controller]")]
public sealed class FacturesFournisseursController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("ModuleComingSoon");
}
