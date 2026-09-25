using FluentValidation;
using OliveMorocco.Business.DTOs.Vente;

namespace OliveMorocco.Business.Validation.Vente;

public sealed class UpdateDevisClientDtoValidator : AbstractValidator<UpdateDevisClientDto>
{
    public UpdateDevisClientDtoValidator()
    {
        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("Le client est obligatoire.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La date est obligatoire.");

        RuleFor(x => x.DateValidite)
            .NotEmpty().WithMessage("La date de validité est obligatoire.")
            .GreaterThanOrEqualTo(x => x.Date)
            .WithMessage("La date de validité doit être postérieure ou égale à la date du devis.");

        RuleFor(x => x.RemiseGlobale)
            .GreaterThanOrEqualTo(0).WithMessage("La remise globale doit être positive ou nulle.");

        RuleFor(x => x.Lignes)
            .NotEmpty().WithMessage("Le devis doit contenir au moins une ligne.");

        RuleForEach(x => x.Lignes)
            .SetValidator(new CreateDevisClientLigneDtoValidator());
    }
}
