using System.ComponentModel.DataAnnotations;

namespace OliveMorocco.Web.Models.Stockage.Stock;

public sealed class StockAjustementViewModel
{
    [Display(Name = "Variation")]
    public decimal? Variation { get; set; }

    [Display(Name = "Note")]
    [MaxLength(500)]
    public string? Note { get; set; }
}
