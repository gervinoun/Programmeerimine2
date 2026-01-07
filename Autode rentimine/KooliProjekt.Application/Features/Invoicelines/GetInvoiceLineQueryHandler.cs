using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class GetInvoiceLineQueryHandler : IRequestHandler<GetInvoiceLineQuery, OperationResult<object>>
    {
        private readonly IInvoiceLineRepository _repo;

        public GetInvoiceLineQueryHandler(IInvoiceLineRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult<object>> Handle(GetInvoiceLineQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            result.Value = await _repo.Query()
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
