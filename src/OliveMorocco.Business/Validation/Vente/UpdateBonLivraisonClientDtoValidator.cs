using FluentValidation;
using OliveMorocco.Business.DTOs.Vente;

namespace OliveMorocco.Business.Validation.Vente;

public sealed class UpdateBonLivraisonClientDtoValidator : AbstractValidator<UpdateBonLivraisonClientDto>
{
    public UpdateBonLivraisonClientDtoValidator()
    {
        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("Le client est obligatoire.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La date est obligatoire.");

        RuleFor(x => x.Lignes)
            .NotEmpty().WithMessage("Le bon de livraison doit contenir au moins une ligne.");

        RuleForEach(x => x.Lignes)
            .SetValidator(new CreateBonLivraisonClientLigneDtoValidator());
    }
}
