using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int MaxPageSize = 100;
        private readonly IInvoiceRepository _repo;

        public ListInvoicesQueryHandler(IInvoiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<IList<Invoice>> Handle(
            ListInvoicesQuery request,
            CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Page <= 0)
                throw new ArgumentException("Page peab olema suurem kui 0.", nameof(request.Page));

            if (request.PageSize <= 0)
                throw new ArgumentException("PageSize peab olema suurem kui 0.", nameof(request.PageSize));

            if (request.PageSize > MaxPageSize)
                throw new ArgumentException($"PageSize ei tohi olla suurem kui {MaxPageSize}.", nameof(request.PageSize));

            var invoices = _repo.Query();
            if (request.BookingId.HasValue)
            {
                invoices = invoices.Where(invoice => invoice.BookingId == request.BookingId.Value);
            }

            if (request.InvoiceDate.HasValue)
            {
                invoices = invoices.Where(invoice => invoice.InvoiceDate == request.InvoiceDate.Value);
            }

            if (request.Total.HasValue)
            {
                invoices = invoices.Where(invoice => invoice.Total == request.Total.Value);
            }

            return await invoices
                .OrderBy(x => x.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }
    }
}