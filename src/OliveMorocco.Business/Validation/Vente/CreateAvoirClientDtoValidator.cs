using FluentValidation;
using OliveMorocco.Business.DTOs.Vente;

namespace OliveMorocco.Business.Validation.Vente;

public sealed class CreateAvoirClientDtoValidator : AbstractValidator<CreateAvoirClientDto>
{
    public CreateAvoirClientDtoValidator()
    {
        RuleFor(x => x.Numero)
            .MaximumLength(50).WithMessage("Le numéro est trop long.");

        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("Le client est obligatoire.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La date est obligatoire.");

        RuleFor(x => x.Motif)
            .NotEmpty().WithMessage("Le motif est obligatoire.");

        RuleFor(x => x.Lignes)
            .NotEmpty().WithMessage("L'avoir doit contenir au moins une ligne.");

        RuleForEach(x => x.Lignes)
            .SetValidator(new CreateAvoirClientLigneDtoValidator());
    }
}
