using FluentValidation;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Validation.Operationnel;

public class CreateRemplissageLigneDtoValidator : AbstractValidator<CreateRemplissageLigneDto>
{
    public CreateRemplissageLigneDtoValidator()
    {
        RuleFor(x => x.ProduitId)
            .GreaterThan(0).WithMessage("Sélectionnez un produit.");

        RuleFor(x => x.Quantite)
            .GreaterThan(0).WithMessage("Le nombre d'unités doit être supérieur à 0.")
            .Must(q => q % 1 == 0).WithMessage("Le nombre d'unités doit être un nombre entier.");
    }
}
