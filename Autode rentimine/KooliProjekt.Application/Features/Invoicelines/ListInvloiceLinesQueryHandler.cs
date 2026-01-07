using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class ListInvoiceLinesQueryHandler : IRequestHandler<ListInvoiceLinesQuery, IList<InvoiceLine>>
    {
        private readonly IInvoiceLineRepository _repo;

        public ListInvoiceLinesQueryHandler(IInvoiceLineRepository repo)
        {
            _repo = repo;
        }

        public async Task<IList<InvoiceLine>> Handle(ListInvoiceLinesQuery request, CancellationToken cancellationToken)
        {
            return await _repo.Query().ToListAsync(cancellationToken);
        }
    }
}
