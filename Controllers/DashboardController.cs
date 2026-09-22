using Microsoft.AspNetCore.Mvc;

namespace OliveMorocco.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
