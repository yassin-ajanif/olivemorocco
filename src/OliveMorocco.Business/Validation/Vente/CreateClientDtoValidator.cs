using FluentValidation;
using OliveMorocco.Business.DTOs.Vente;

namespace OliveMorocco.Business.Validation.Vente;

public class CreateClientDtoValidator : AbstractValidator<CreateClientDto>
{
    public CreateClientDtoValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(200).WithMessage("Le nom ne doit pas dépasser 200 caractères.");

        RuleFor(x => x.Adresse)
            .NotEmpty().WithMessage("L'adresse est obligatoire.")
            .MaximumLength(500);

        RuleFor(x => x.Ville)
            .NotEmpty().WithMessage("La ville est obligatoire.")
            .MaximumLength(128);

        RuleFor(x => x.Telephone)
            .NotEmpty().WithMessage("Le téléphone est obligatoire.")
            .MaximumLength(32);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'e-mail est obligatoire.")
            .EmailAddress().WithMessage("L'adresse e-mail n'est pas valide.")
            .MaximumLength(256);

        RuleFor(x => x.ICE)
            .NotEmpty().WithMessage("L'ICE est obligatoire.")
            .Length(15).WithMessage("L'ICE doit comporter exactement 15 caractères.");

        RuleFor(x => x.ConditionsPaiement)
            .NotEmpty().WithMessage("Les conditions de paiement sont obligatoires.")
            .MaximumLength(256);
    }
}
