using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Invoices
{
    public class SaveInvoiceCommandHandler : IRequestHandler<SaveInvoiceCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public SaveInvoiceCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(SaveInvoiceCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            var invoice = new Invoice();
            if (request.Id == 0)
            {
                await _dbContext.Invoices.AddAsync(invoice, cancellationToken);
            }
            else
            {
                invoice = await _dbContext.Invoices.FindAsync(
                    new object[] { request.Id }, cancellationToken);
            }

            invoice.BookingId = request.BookingId;
            invoice.InvoiceDate = request.InvoiceDate;
            invoice.Total = request.Total;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
