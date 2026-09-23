using FluentValidation;
using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Business.Validation.Achat;

public sealed class UpdateFactureFournisseurDtoValidator : AbstractValidator<UpdateFactureFournisseurDto>
{
    public UpdateFactureFournisseurDtoValidator()
    {
        RuleFor(x => x.FournisseurId)
            .GreaterThan(0).WithMessage("Le fournisseur est obligatoire.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La date est obligatoire.");

        RuleFor(x => x.DateEcheance)
            .NotEmpty().WithMessage("La date d'échéance est obligatoire.")
            .GreaterThanOrEqualTo(x => x.Date)
            .WithMessage("La date d'échéance doit être postérieure ou égale à la date de facture.");

        RuleFor(x => x.RemiseGlobale)
            .GreaterThanOrEqualTo(0).WithMessage("La remise globale doit être positive ou nulle.");

        RuleFor(x => x.Lignes)
            .NotEmpty().WithMessage("La facture doit contenir au moins une ligne.");

        RuleForEach(x => x.Lignes)
            .SetValidator(new CreateFactureFournisseurLigneDtoValidator());
    }
}
