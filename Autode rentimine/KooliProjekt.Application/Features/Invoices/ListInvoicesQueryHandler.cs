using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class ListInvoiceLinesQueryHandler
        : IRequestHandler<ListInvoiceLinesQuery, IList<InvoiceLine>>
    {
        private readonly ApplicationDbContext _db;

        public ListInvoiceLinesQueryHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IList<InvoiceLine>> Handle(
            ListInvoiceLinesQuery request,
            CancellationToken cancellationToken)
        {
            return await _db.InvoiceLines.ToListAsync(cancellationToken);
        }
    }
}
