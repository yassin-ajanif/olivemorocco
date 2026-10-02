using FluentValidation;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Validation.Operationnel;

public class UpdatePressageDtoValidator : AbstractValidator<UpdatePressageDto>
{
    public UpdatePressageDtoValidator()
    {
        RuleFor(x => x.FournisseurId)
            .GreaterThan(0).WithMessage("Sélectionnez une huilerie.");

        RuleFor(x => x.VarieteId)
            .GreaterThan(0).WithMessage("Sélectionnez une variété.");

        RuleFor(x => x.QuantiteOlives)
            .GreaterThan(0).WithMessage("La quantité d'olives doit être supérieure à zéro.");

        // Rendement is derived from the two measured quantities by the service, not entered,
        // so it is no longer validated here. The oil yield is the required input instead —
        // it is the figure the mill reports, and without it there is nothing to divide.
        RuleFor(x => x.QuantiteHuile)
            .NotNull().WithMessage("La quantité d'huile est requise pour calculer le rendement.")
            .GreaterThan(0).WithMessage("La quantité d'huile doit être supérieure à zéro.")
            .When(x => x.QuantiteHuile.HasValue);

        // More oil than olives is arithmetically fine but physically impossible, and it is
        // the signature of the two fields having been swapped.
        RuleFor(x => x.QuantiteHuile)
            .LessThanOrEqualTo(x => x.QuantiteOlives)
            .When(x => x.QuantiteHuile.HasValue && x.QuantiteOlives > 0)
            .WithMessage("La quantité d'huile ne peut pas dépasser la quantité d'olives.");

        RuleFor(x => x.TypeChargeId)
            .GreaterThan(0).WithMessage("Sélectionnez un type de charge.");

        RuleFor(x => x.Libelle)
            .NotEmpty().WithMessage("Le libellé de la charge est requis.");

        RuleFor(x => x.MontantTtc)
            .GreaterThanOrEqualTo(0).WithMessage("Le montant de la charge ne peut pas être négatif.");
    }
}
