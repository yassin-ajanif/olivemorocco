using FluentValidation;
using OliveMorocco.Business.DTOs.Achat;

namespace OliveMorocco.Business.Validation.Achat;

public sealed class UpdateAvoirFournisseurDtoValidator : AbstractValidator<UpdateAvoirFournisseurDto>
{
    public UpdateAvoirFournisseurDtoValidator()
    {
        RuleFor(x => x.FournisseurId)
            .GreaterThan(0).WithMessage("Le fournisseur est obligatoire.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La date est obligatoire.");

        RuleFor(x => x.Motif)
            .NotEmpty().WithMessage("Le motif est obligatoire.");

        RuleFor(x => x.Lignes)
            .NotEmpty().WithMessage("L'avoir doit contenir au moins une ligne.");

        RuleForEach(x => x.Lignes)
            .SetValidator(new CreateAvoirFournisseurLigneDtoValidator());
    }
}
