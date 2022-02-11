using FluentValidation;
using OneEstate.Application.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace OneEstate.Application.Services.Validation.Project
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectInput>
    {
        private List<string> ProjectTypes = new List<string> { "development", "rent" };

        public CreateProjectValidator()
        {
            ValidateName();
            ValidateType();
            ValidateShortDescription();
            ValidateDescription();
            ValidateImages();
        }

        private void ValidateName()
        {
            RuleFor(a => a.Name).MaximumLength(60).WithMessage("Numele trebuie sa aiba maxim 60 de caractere.");
            RuleFor(a => a.Name).MinimumLength(10).WithMessage("Numele trebuie sa aiba minim 10 de caractere.");
            RuleFor(a => a.Name).NotEmpty().WithMessage("Numele trebuie completat.");
        }

        private void ValidateType()
        {
            RuleFor(a => a.ProjectType).Must(x => ProjectTypes.Contains(x)).WithMessage("Tipul proiectului este invalid.");
        }

        private void ValidateShortDescription()
        {
            RuleFor(a => a.ShortDescription).MaximumLength(100).WithMessage("Scurta descriere trebuie sa aiba maxim 100 de caractere.");
            RuleFor(a => a.ShortDescription).MinimumLength(10).WithMessage("Scurta descriere trebuie sa aiba minim 10 de caractere.");
            RuleFor(a => a.ShortDescription).NotEmpty().WithMessage("Scurta descriere trebuie completata.");
        }

        private void ValidateDescription()
        {
            RuleFor(a => a.Description).MaximumLength(8000).WithMessage("Descrierea trebuie sa aiba maxim 8000 de caractere.");
            RuleFor(a => a.Description).MinimumLength(100).WithMessage("Descrierea trebuie sa aiba minim 100 de caractere.");
            RuleFor(a => a.Description).NotEmpty().WithMessage("Descrierea trebuie completata.");
        }

        private void ValidateImages()
        {
            RuleFor(a => a).Must(a => a.ImageIds.Any()).WithMessage("Cel putin o imagine trebuie adaugata");
        }
    }
}
