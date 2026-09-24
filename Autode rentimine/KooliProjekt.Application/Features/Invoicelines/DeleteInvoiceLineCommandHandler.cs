using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class DeleteInvoiceLineCommandHandler
        : IRequestHandler<DeleteInvoiceLineCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteInvoiceLineCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext
                ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult> Handle(
            DeleteInvoiceLineCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult();

            if (request.Id <= 0)
            {
                return result;
            }

            var invoiceLine = await _dbContext.InvoiceLines
                .FindAsync(new object[] { request.Id }, cancellationToken);

            if (invoiceLine == null)
            {
                return result;
            }

            _dbContext.InvoiceLines.Remove(invoiceLine);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}