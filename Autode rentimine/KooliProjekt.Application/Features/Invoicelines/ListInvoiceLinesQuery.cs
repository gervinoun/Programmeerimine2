using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class ListInvoiceLinesQuery : IRequest<IList<InvoiceLine>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}