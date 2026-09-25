using FluentValidation;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Validation.Operationnel;

public class CreatePressageDtoValidator : AbstractValidator<CreatePressageDto>
{
    public CreatePressageDtoValidator()
    {
        RuleFor(x => x.FournisseurId)
            .GreaterThan(0).WithMessage("Sélectionnez une huilerie.");

        RuleFor(x => x.VarieteId)
            .GreaterThan(0).WithMessage("Sélectionnez une variété.");

        RuleFor(x => x.QuantiteOlives)
            .GreaterThan(0).WithMessage("La quantité d'olives doit être supérieure à zéro.");

        RuleFor(x => x.Rendement)
            .GreaterThan(0).WithMessage("Le rendement doit être supérieur à zéro.")
            .LessThanOrEqualTo(100).WithMessage("Le rendement ne peut pas dépasser 100 %.");

        RuleFor(x => x.QuantiteHuile)
            .GreaterThanOrEqualTo(0).When(x => x.QuantiteHuile.HasValue)
            .WithMessage("La quantité d'huile ne peut pas être négative.");
    }
}
