using FluentValidation;

namespace KooliProjekt.Application.Features.Cars
{
    public class SaveCarCommandValidator : AbstractValidator<SaveCarCommand>
    {
        public SaveCarCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Id must be zero or greater");

            RuleFor(x => x.NumberPlate)
                .NotEmpty()
                .WithMessage("Number plate is required");

            RuleFor(x => x.TypeId)
                .GreaterThan(0)
                .WithMessage("Type id must be greater than zero");

            RuleFor(x => x.Kmrate)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Km rate cannot be negative");

            RuleFor(x => x.TimeRate)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Time rate cannot be negative");
        }
    }
}