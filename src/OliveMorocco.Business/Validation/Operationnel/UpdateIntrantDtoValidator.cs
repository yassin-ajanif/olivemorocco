using FluentValidation;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Validation.Operationnel;

public class UpdateIntrantDtoValidator : AbstractValidator<UpdateIntrantDto>
{
    public UpdateIntrantDtoValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(128).WithMessage("Le nom ne doit pas dépasser 128 caractères.");

        RuleFor(x => x.Unite)
            .NotEmpty().WithMessage("L'unité est obligatoire.")
            .MaximumLength(16).WithMessage("L'unité ne doit pas dépasser 16 caractères.");
    }
}
