using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int MaxPageSize = 100;
        private readonly IInvoiceLineRepository _repo;

        public ListInvoiceLinesQueryHandler(IInvoiceLineRepository repo)
        {
            _repo = repo;
        }

        public async Task<IList<InvoiceLine>> Handle(
            ListInvoiceLinesQuery request,
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

            return await _repo.Query()
                .OrderBy(x => x.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }
    }
}