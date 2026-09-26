using FluentValidation;
using OliveMorocco.Business.DTOs.Common;

namespace OliveMorocco.Business.Validation.Common;

public sealed class CreateFacturePaiementDtoValidator : AbstractValidator<CreateFacturePaiementDto>
{
    public CreateFacturePaiementDtoValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("La date du paiement est obligatoire.");

        RuleFor(x => x.Montant)
            .GreaterThan(0).WithMessage("Le montant doit être positif.");

        RuleFor(x => x.Mode)
            .IsInEnum().WithMessage("Le mode de paiement est invalide.");

        RuleFor(x => x.Reference)
            .MaximumLength(128).WithMessage("La référence ne peut pas dépasser 128 caractères.");
    }
}
