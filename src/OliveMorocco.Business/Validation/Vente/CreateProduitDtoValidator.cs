using FluentValidation;
using OliveMorocco.Business.DTOs.Vente;

namespace OliveMorocco.Business.Validation.Vente;

public class CreateProduitDtoValidator : AbstractValidator<CreateProduitDto>
{
    public CreateProduitDtoValidator()
    {
        RuleFor(x => x.Reference)
            .NotEmpty().WithMessage("La référence est obligatoire.")
            .MaximumLength(64).WithMessage("La référence ne doit pas dépasser 64 caractères.");

        RuleFor(x => x.Designation)
            .NotEmpty().WithMessage("La désignation est obligatoire.")
            .MaximumLength(256).WithMessage("La désignation ne doit pas dépasser 256 caractères.");

        RuleFor(x => x.Unite)
            .NotEmpty().WithMessage("L'unité est obligatoire.")
            .MaximumLength(32).WithMessage("L'unité ne doit pas dépasser 32 caractères.");

        RuleFor(x => x.CodeBarre)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.CodeBarre));

        RuleFor(x => x.PrixAchatHT)
            .GreaterThanOrEqualTo(0).WithMessage("Le prix d'achat doit être positif ou nul.");

        RuleFor(x => x.PrixVenteHT)
            .GreaterThanOrEqualTo(0).WithMessage("Le prix de vente doit être positif ou nul.");

        RuleFor(x => x.TauxTVA)
            .GreaterThanOrEqualTo(0).WithMessage("Le taux de TVA doit être positif ou nul.");

        RuleFor(x => x.StockInitial)
            .GreaterThanOrEqualTo(0).WithMessage("Le stock initial doit être positif ou nul.");

        RuleFor(x => x.StockMinimum)
            .GreaterThanOrEqualTo(0).WithMessage("Le stock minimum doit être positif ou nul.");
    }
}
