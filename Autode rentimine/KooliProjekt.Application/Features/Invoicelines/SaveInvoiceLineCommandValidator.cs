using FluentValidation;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class SaveInvoiceLineCommandValidator : AbstractValidator<SaveInvoiceLineCommand>
    {
        public SaveInvoiceLineCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Id must be zero or greater");

            RuleFor(x => x.InvoiceId)
                .GreaterThan(0)
                .WithMessage("Invoice id must be greater than zero");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Amount cannot be negative");
        }
    }
}