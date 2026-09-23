using FluentValidation;
using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Business.Validation.Achat;

public class CreateFournisseurDtoValidator : AbstractValidator<CreateFournisseurDto>
{
    public CreateFournisseurDtoValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(200).WithMessage("Le nom ne doit pas dépasser 200 caractères.");

        RuleFor(x => x.Adresse).MaximumLength(500);
        RuleFor(x => x.Ville).MaximumLength(128);
        RuleFor(x => x.Telephone).MaximumLength(32);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("L'adresse e-mail n'est pas valide.")
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.ICE)
            .Length(15).WithMessage("L'ICE doit comporter exactement 15 caractères.")
            .When(x => !string.IsNullOrWhiteSpace(x.ICE));

        RuleFor(x => x.ConditionsPaiement).MaximumLength(256);
    }
}
