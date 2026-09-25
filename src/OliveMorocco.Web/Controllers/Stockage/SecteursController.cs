using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class SecteursController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();
}
