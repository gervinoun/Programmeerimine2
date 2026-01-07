using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class SaveInvoiceLineCommandHandler
        : IRequestHandler<SaveInvoiceLineCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public SaveInvoiceLineCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(
            SaveInvoiceLineCommand request,
            CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            var line = new InvoiceLine();
            if (request.Id == 0)
            {
                await _dbContext.InvoiceLines.AddAsync(line, cancellationToken);
            }
            else
            {
                line = await _dbContext.InvoiceLines.FindAsync(
                    new object[] { request.Id }, cancellationToken);
            }

            line.InvoiceId = request.InvoiceId;
            line.Description = request.Description;
            line.Amount = request.Amount;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
