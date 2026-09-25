using FluentValidation;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Validation.Stockage;

public sealed class CreateAjustementStockDtoValidator : AbstractValidator<CreateAjustementStockDto>
{
    public CreateAjustementStockDtoValidator()
    {
        RuleFor(x => x.Variation)
            .NotEqual(0).WithMessage("La variation doit être différente de zéro.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("La note ne peut pas dépasser 500 caractères.");
    }
}
