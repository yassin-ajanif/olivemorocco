using FluentValidation;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Validation.Operationnel;

public class CreateVarieteDtoValidator : AbstractValidator<CreateVarieteDto>
{
    public CreateVarieteDtoValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom de la variété est obligatoire.")
            .MaximumLength(128).WithMessage("Le nom ne doit pas dépasser 128 caractères.");

        RuleFor(x => x.Code)
            .MaximumLength(32).WithMessage("Le code ne doit pas dépasser 32 caractères.")
            .When(x => !string.IsNullOrWhiteSpace(x.Code));

        RuleFor(x => x.RegionOrigine)
            .MaximumLength(128).WithMessage("La région ne doit pas dépasser 128 caractères.")
            .When(x => !string.IsNullOrWhiteSpace(x.RegionOrigine));
    }
}
