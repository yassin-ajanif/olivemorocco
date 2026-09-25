using FluentValidation;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Validation.Operationnel;

public class CreateInterventionDtoValidator : AbstractValidator<CreateInterventionDto>
{
    public CreateInterventionDtoValidator()
    {
        RuleFor(x => x.SecteurId)
            .GreaterThan(0).WithMessage("Sélectionnez un secteur.");

        RuleForEach(x => x.Lignes).SetValidator(new CreateInterventionLigneDtoValidator());

        RuleForEach(x => x.Charges).SetValidator(new CreateInterventionChargeDtoValidator());

        RuleFor(x => x.Lignes)
            .Must(lignes => lignes.Select(l => l.IntrantId).Distinct().Count() == lignes.Count)
            .When(x => x.Lignes.Count > 0)
            .WithMessage("Chaque intrant ne peut apparaître qu'une seule fois par intervention.");

        RuleFor(x => x.QuantiteEau)
            .GreaterThanOrEqualTo(0).When(x => x.QuantiteEau.HasValue)
            .WithMessage("La quantité d'eau ne peut pas être négative.");

        RuleFor(x => x.Note).MaximumLength(2000);
    }
}
