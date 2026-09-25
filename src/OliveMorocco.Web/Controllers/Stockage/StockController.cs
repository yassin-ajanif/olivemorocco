using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Routing;

namespace OliveMorocco.Web.Controllers.Stockage;

[Route(AppSections.Stockage + "/[controller]")]
public sealed class StockController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();
}
