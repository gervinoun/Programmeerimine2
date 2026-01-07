using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.InvoiceLines
{

    public class DeleteInvoiceLineCommandHandler
        : IRequestHandler<DeleteInvoiceLineCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteInvoiceLineCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(
            DeleteInvoiceLineCommand request,
            CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            await _dbContext
                .InvoiceLines
                .Where(x => x.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            return result;
        }
    }
}
