using FluentValidation;

namespace KooliProjekt.Application.Features.Users
{
    public class SaveUserCommandValidator : AbstractValidator<SaveUserCommand>
    {
        public SaveUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Id must be zero or greater");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email must be valid");

            RuleFor(x => x.PasswordHash)
                .NotEmpty()
                .WithMessage("Password is required");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Phone is required");
        }
    }
}