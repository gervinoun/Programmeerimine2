using FluentValidation;

namespace KooliProjekt.Application.Features.CarTypes
{
    public class SaveCarTypeCommandValidator : AbstractValidator<SaveCarTypeCommand>
    {
        public SaveCarTypeCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Id must be zero or greater");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required");
        }
    }
}