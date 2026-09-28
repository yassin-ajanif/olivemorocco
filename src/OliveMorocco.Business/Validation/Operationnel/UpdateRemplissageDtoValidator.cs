using FluentValidation;
using OliveMorocco.Business.DTOs.Operationnel;

namespace OliveMorocco.Business.Validation.Operationnel;

public class UpdateRemplissageDtoValidator : AbstractValidator<UpdateRemplissageDto>
{
    public UpdateRemplissageDtoValidator()
    {
        RuleFor(x => x.VarieteId)
            .GreaterThan(0).WithMessage("Sélectionnez une variété.");

        RuleFor(x => x.Perte)
            .GreaterThanOrEqualTo(0).WithMessage("La perte ne peut pas être négative.");

        RuleFor(x => x.Note).MaximumLength(500);

        RuleFor(x => x.Lignes)
            .NotEmpty().WithMessage("Ajoutez au moins un produit à remplir.");

        RuleForEach(x => x.Lignes).SetValidator(new CreateRemplissageLigneDtoValidator());

        RuleFor(x => x.Lignes)
            .Must(lignes => lignes.Select(l => l.ProduitId).Distinct().Count() == lignes.Count)
            .When(x => x.Lignes.Count > 0)
            .WithMessage("Chaque produit ne peut apparaître qu'une seule fois par remplissage.");
    }
}
