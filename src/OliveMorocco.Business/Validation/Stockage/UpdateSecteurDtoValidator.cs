using FluentValidation;
using OliveMorocco.Business.DTOs.Stockage;

namespace OliveMorocco.Business.Validation.Stockage;

public class UpdateSecteurDtoValidator : AbstractValidator<UpdateSecteurDto>
{
    public UpdateSecteurDtoValidator()
    {
        RuleFor(x => x.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(128).WithMessage("Le nom ne doit pas dépasser 128 caractères.");

        RuleFor(x => x.Code)
            .MaximumLength(32).WithMessage("Le code ne doit pas dépasser 32 caractères.");

        RuleFor(x => x.SuperficieHectares)
            .GreaterThan(0).WithMessage("La superficie totale doit être supérieure à 0.");

        RuleForEach(x => x.Lignes).SetValidator(new CreateSecteurVarieteLineDtoValidator());

        RuleFor(x => x.Lignes)
            .Must(lignes => lignes.Select(l => l.VarieteId).Distinct().Count() == lignes.Count)
            .When(x => x.Lignes.Count > 0)
            .WithMessage("Chaque variété ne peut apparaître qu'une seule fois par secteur.");
    }
}
