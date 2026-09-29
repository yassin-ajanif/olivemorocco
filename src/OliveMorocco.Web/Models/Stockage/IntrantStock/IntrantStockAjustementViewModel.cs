using System.ComponentModel.DataAnnotations;

namespace OliveMorocco.Web.Models.Stockage.IntrantStock;

public sealed class IntrantStockAjustementViewModel
{
    [Display(Name = "Variation")]
    public decimal? Variation { get; set; }

    [Display(Name = "Note")]
    [MaxLength(500)]
    public string? Note { get; set; }
}
