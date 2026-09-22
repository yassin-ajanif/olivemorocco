using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Vente;

[Route(AppSections.Vente + "/[controller]")]
public sealed class BonsLivraisonController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("ModuleComingSoon");
}
