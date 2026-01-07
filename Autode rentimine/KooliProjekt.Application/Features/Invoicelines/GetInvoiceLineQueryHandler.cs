using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class GetInvoiceLineQueryHandler
        : IRequestHandler<GetInvoiceLineQuery, OperationResult<object>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetInvoiceLineQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<object>> Handle(
            GetInvoiceLineQuery request,
            CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            result.Value = await _dbContext
                .InvoiceLines
                .Where(l => l.Id == request.Id)
                .Select(l => new
                {
                    l.Id,
                    l.InvoiceId,
                    l.Description,
                    l.Amount
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }
}
