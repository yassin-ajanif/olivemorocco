using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;

namespace OliveMorocco.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Route("Home/Vitrine")]
    public IActionResult Vitrine()
    {
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

        if (feature?.Error is { } ex)
        {
            _logger.LogError(
                ex,
                "Unhandled error. RequestId={RequestId} Path={Path}",
                requestId,
                feature.Path);
        }

        return View(new ErrorViewModel { RequestId = requestId });
    }
}
