using FluentValidation;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Validation.Operationnel;

public class CreateInterventionChargeDtoValidator : AbstractValidator<CreateInterventionChargeDto>
{
    public CreateInterventionChargeDtoValidator()
    {
        RuleFor(x => x.TypeChargeId)
            .GreaterThan(0).WithMessage("Sélectionnez un type de charge.");

        RuleFor(x => x.Libelle)
            .NotEmpty().WithMessage("Le libellé est obligatoire.")
            .MaximumLength(256);

        RuleFor(x => x.MontantTtc)
            .GreaterThan(0).WithMessage("Le montant TTC doit être positif.");

        RuleFor(x => x.Note).MaximumLength(2000);
    }
}
