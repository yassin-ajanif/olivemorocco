using FluentValidation;
using OliveMorocco.Business.DTOs.Vente;
using OliveMorocco.Business.Validation.Common;

namespace OliveMorocco.Business.Validation.Vente;

public sealed class UpdateFactureClientDtoValidator : AbstractValidator<UpdateFactureClientDto>
{
    public UpdateFactureClientDtoValidator()
    {
        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("Le client est obligatoire.");

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
            .SetValidator(new CreateFactureClientLigneDtoValidator());

        RuleForEach(x => x.Paiements)
            .SetValidator(new CreateFacturePaiementDtoValidator());
    }
}
