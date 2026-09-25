using FluentValidation;
using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Business.Validation.Achat;

public sealed class CreateBonReceptionLigneDtoValidator : AbstractValidator<CreateBonReceptionLigneDto>
{
    public CreateBonReceptionLigneDtoValidator()
    {
        RuleFor(x => x.IntrantId)
            .GreaterThan(0).WithMessage("Chaque ligne doit référencer un produit.");

        RuleFor(x => x.Designation)
            .NotEmpty().WithMessage("La désignation est obligatoire.");

        RuleFor(x => x.QuantiteRecue)
            .GreaterThan(0).WithMessage("La quantité reçue doit être supérieure à zéro.");

        RuleFor(x => x.PrixUnitaireHT)
            .GreaterThanOrEqualTo(0).WithMessage("Le prix unitaire HT doit être positif ou nul.");

        RuleFor(x => x.TauxTVA)
            .GreaterThanOrEqualTo(0).WithMessage("Le taux de TVA doit être positif ou nul.");
    }
}
