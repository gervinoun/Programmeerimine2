using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Invoices
{
    public class ListInvoicesQueryHandler
        : IRequestHandler<ListInvoicesQuery, IList<Invoice>>
    {
        private readonly ApplicationDbContext _db;

        public ListInvoicesQueryHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IList<Invoice>> Handle(
            ListInvoicesQuery request,
            CancellationToken cancellationToken)
        {
            return await _db.Invoices.ToListAsync(cancellationToken);
        }
    }
}
