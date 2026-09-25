using FluentValidation;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Validation.Stockage;

public class CreateSecteurVarieteLineDtoValidator : AbstractValidator<CreateSecteurVarieteLineDto>
{
    public CreateSecteurVarieteLineDtoValidator()
    {
        RuleFor(x => x.VarieteId)
            .GreaterThan(0).WithMessage("Sélectionnez une variété.");

        RuleFor(x => x.SuperficieHectares)
            .GreaterThan(0).WithMessage("La superficie allouée doit être supérieure à 0.");
    }
}
