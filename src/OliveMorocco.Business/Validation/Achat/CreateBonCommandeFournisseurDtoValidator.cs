using FluentValidation;
using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Business.Validation.Achat;

public sealed class CreateBonCommandeFournisseurDtoValidator : AbstractValidator<CreateBonCommandeFournisseurDto>
{
    public CreateBonCommandeFournisseurDtoValidator()
    {
        RuleFor(x => x.Numero)
            .MaximumLength(50).WithMessage("Le numéro est trop long.");

        RuleFor(x => x.FournisseurId)
            .GreaterThan(0).WithMessage("Le fournisseur est obligatoire.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La date est obligatoire.");

        RuleFor(x => x.Lignes)
            .NotEmpty().WithMessage("Le bon de commande doit contenir au moins une ligne.");

        RuleForEach(x => x.Lignes)
            .SetValidator(new CreateBonCommandeFournisseurLigneDtoValidator());
    }
}
