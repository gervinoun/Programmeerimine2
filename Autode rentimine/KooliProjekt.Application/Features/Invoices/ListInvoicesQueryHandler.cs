using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Invoices
{
    public class ListInvoicesQueryHandler : IRequestHandler<ListInvoicesQuery, IList<Invoice>>
    {
        private readonly IInvoiceRepository _repo;

        public ListInvoicesQueryHandler(IInvoiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<IList<Invoice>> Handle(ListInvoicesQuery request, CancellationToken cancellationToken)
        {
            return await _repo.Query().ToListAsync(cancellationToken);
        }
    }
}
