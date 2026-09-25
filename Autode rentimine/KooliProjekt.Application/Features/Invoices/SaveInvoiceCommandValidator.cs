using FluentValidation;

namespace KooliProjekt.Application.Features.Invoices
{
    public class SaveInvoiceCommandValidator : AbstractValidator<SaveInvoiceCommand>
    {
        public SaveInvoiceCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Id must be zero or greater");

            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage("Booking id must be greater than zero");

            RuleFor(x => x.InvoiceDate)
                .NotEmpty()
                .WithMessage("Invoice date is required");

            RuleFor(x => x.Total)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total cannot be negative");
        }
    }
}