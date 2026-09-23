using FluentValidation;
using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Business.Validation.Achat;

public sealed class UpdateBonReceptionDtoValidator : AbstractValidator<UpdateBonReceptionDto>
{
    public UpdateBonReceptionDtoValidator()
    {
        RuleFor(x => x.FournisseurId)
            .GreaterThan(0).WithMessage("Le fournisseur est obligatoire.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La date est obligatoire.");

        RuleFor(x => x.Lignes)
            .NotEmpty().WithMessage("Le bon de réception doit contenir au moins une ligne.");

        RuleForEach(x => x.Lignes)
            .SetValidator(new CreateBonReceptionLigneDtoValidator());
    }
}
