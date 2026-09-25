using FluentValidation;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Validation.Operationnel;

public class CreateInterventionLigneDtoValidator : AbstractValidator<CreateInterventionLigneDto>
{
    public CreateInterventionLigneDtoValidator()
    {
        RuleFor(x => x.IntrantId)
            .GreaterThan(0).WithMessage("Sélectionnez un intrant.");

        RuleFor(x => x.Quantite)
            .GreaterThan(0).WithMessage("Indiquez une quantité supérieure à zéro.");
    }
}
