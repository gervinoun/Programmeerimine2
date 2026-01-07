using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Invoices
{
    public class GetInvoiceQueryHandler : IRequestHandler<GetInvoiceQuery, OperationResult<object>>
    {
        private readonly IInvoiceRepository _repo;

        public GetInvoiceQueryHandler(IInvoiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult<object>> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            result.Value = await _repo.Query()
                .Where(i => i.Id == request.Id)
                .Select(i => new
                {
                    i.Id,
                    i.BookingId,
                    i.InvoiceDate,
                    i.Total
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }
}
