using FluentValidation;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Validation.Operationnel;

public class UpdateInterventionDtoValidator : AbstractValidator<UpdateInterventionDto>
{
    public UpdateInterventionDtoValidator()
    {
        RuleFor(x => x.SecteurId)
            .GreaterThan(0).WithMessage("Sélectionnez un secteur.");

        RuleFor(x => x.IntrantId)
            .GreaterThan(0).When(x => x.QuantiteIntrant.HasValue)
            .WithMessage("Sélectionnez un intrant lorsque la quantité est renseignée.");

        RuleFor(x => x.QuantiteIntrant)
            .GreaterThan(0).When(x => x.IntrantId.HasValue)
            .WithMessage("Indiquez la quantité d'intrant utilisée.");

        RuleFor(x => x.QuantiteEau)
            .GreaterThanOrEqualTo(0).When(x => x.QuantiteEau.HasValue)
            .WithMessage("La quantité d'eau ne peut pas être négative.");

        RuleFor(x => x.Note).MaximumLength(2000);
    }
}
