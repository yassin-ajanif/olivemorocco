using Microsoft.AspNetCore.Mvc;

namespace OliveMorocco.Web.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Clients");
    }
}
