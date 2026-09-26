using FluentValidation;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Validation.Stockage;

public class CreateIntrantDtoValidator : AbstractValidator<CreateIntrantDto>
{
    public CreateIntrantDtoValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(128).WithMessage("Le nom ne doit pas dépasser 128 caractères.");

        RuleFor(x => x.Unite)
            .NotEmpty().WithMessage("L'unité est obligatoire.")
            .MaximumLength(16).WithMessage("L'unité ne doit pas dépasser 16 caractères.");

        RuleFor(x => x.PrixAchatHT)
            .GreaterThanOrEqualTo(0).WithMessage("Le PU HT doit être positif ou nul.");
    }
}
