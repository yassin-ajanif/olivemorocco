using FluentValidation;
using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Business.Validation.Achat;

public sealed class CreateBonCommandeFournisseurLigneDtoValidator : AbstractValidator<CreateBonCommandeFournisseurLigneDto>
{
    public CreateBonCommandeFournisseurLigneDtoValidator()
    {
        RuleFor(x => x)
            .Must(l => (l.IntrantId is > 0) ^ (l.ServiceId is > 0))
            .WithMessage("Chaque ligne doit référencer un produit ou un service, mais pas les deux.");

        RuleFor(x => x.Designation)
            .NotEmpty().WithMessage("La désignation est obligatoire.");

        RuleFor(x => x.QuantiteCommandee)
            .GreaterThan(0).WithMessage("La quantité commandée doit être supérieure à zéro.");

        RuleFor(x => x.PrixUnitaireHT)
            .GreaterThanOrEqualTo(0).WithMessage("Le prix unitaire HT doit être positif ou nul.");

        RuleFor(x => x.Remise)
            .GreaterThanOrEqualTo(0).WithMessage("La remise doit être positive ou nulle.");

        RuleFor(x => x.TauxTVA)
            .GreaterThanOrEqualTo(0).WithMessage("Le taux de TVA doit être positif ou nul.");
    }
}
