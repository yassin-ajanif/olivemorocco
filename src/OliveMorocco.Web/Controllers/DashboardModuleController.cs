using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;

namespace OliveMorocco.Web.Controllers;

public abstract class DashboardModuleController : Controller
{
    protected IActionResult ModuleList(ModuleListPageViewModel model)
    {
        ViewData["DashSection"] = model.Section;
        ViewData["DashModule"] = model.ModuleKey;
        ViewData["Title"] = model.Title;
        return View("ModuleList", model);
    }
}
