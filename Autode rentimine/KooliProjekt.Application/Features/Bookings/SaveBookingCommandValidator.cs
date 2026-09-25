using FluentValidation;

namespace KooliProjekt.Application.Features.Bookings
{
    public class SaveBookingCommandValidator : AbstractValidator<SaveBookingCommand>
    {
        public SaveBookingCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Id must be zero or greater");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("User id must be greater than zero");

            RuleFor(x => x.CarId)
                .GreaterThan(0)
                .WithMessage("Car id must be greater than zero");

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("Start time is required");

            RuleFor(x => x.EndTime)
                .NotEmpty()
                .WithMessage("End time is required")
                .GreaterThan(x => x.StartTime)
                .WithMessage("End time must be after start time");

            RuleFor(x => x.KmStart)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Km start cannot be negative");

            RuleFor(x => x.KmEnd)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Km end cannot be negative");

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required");
        }
    }
}