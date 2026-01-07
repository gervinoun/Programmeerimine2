using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Invoices
{
    public class SaveInvoiceCommandHandler : IRequestHandler<SaveInvoiceCommand, OperationResult>
    {
        private readonly IInvoiceRepository _repo;

        public SaveInvoiceCommandHandler(IInvoiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult> Handle(SaveInvoiceCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            Invoice invoice;
            if (request.Id == 0)
            {
                invoice = new Invoice();
                await _repo.AddAsync(invoice, cancellationToken);
            }
            else
            {
                invoice = await _repo.GetByIdAsync(request.Id, cancellationToken);
            }

            invoice.BookingId = request.BookingId;
            invoice.InvoiceDate = request.InvoiceDate;
            invoice.Total = request.Total;

            await _repo.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
