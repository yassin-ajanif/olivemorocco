using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OliveMorocco.Web.Models;
using OliveMorocco.Web.Models.Boutique;

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
        return RedirectToAction(nameof(Boutique));
    }

    /// <summary>
    /// Public shop grid. Backed by <see cref="BoutiqueDemoCatalogue"/> for now — swap for a
    /// service reading <c>Produits</c> when the real photography and stock are wired up.
    /// </summary>
    [Route("Home/Boutique")]
    public IActionResult Boutique()
    {
        return View(BoutiqueDemoCatalogue.Varietes);
    }

    /// <summary>Public product detail page.</summary>
    [Route("Home/Boutique/{id:int}")]
    public IActionResult BoutiqueDetail(int id)
    {
        var produit = BoutiqueDemoCatalogue.Trouver(id);
        if (produit is null)
            return NotFound();

        var variete = BoutiqueDemoCatalogue.TrouverVariete(produit.VarieteId);

        return View(new BoutiqueProduitDetailViewModel
        {
            Produit = produit,
            Variete = variete,
            Resume = "Première pression à froid, récoltée en septembre sur nos parcelles de "
                   + "Had Touabet. Extraction à froid, sans additif ni chaleur ajoutée, "
                   + "puis conditionnée avec le numéro de lot appliqué à la source.",
            Specs =
            [
                ("Contenance", produit.ContenanceLabel),
                ("Variété", produit.VarieteNom),
                ("Conditionnement", produit.Unite),
                ("Référence", produit.Reference),
                ("Origine", $"AOP Tyout Chiadma — {produit.VarieteRegion}"),
                ("Récolte", "Septembre 2026"),
            ],
            // Same variety, other formats — the buyer can step up or down without
            // losing the oil they were looking at.
            Autres = variete?.Produits.Where(p => p.Id != id).ToList() ?? [],
            // Everything else, so the page still offers a way onward.
            AutresVarietes = BoutiqueDemoCatalogue.Tous
                .Where(p => p.VarieteId != produit.VarieteId)
                .ToList(),
        });
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
