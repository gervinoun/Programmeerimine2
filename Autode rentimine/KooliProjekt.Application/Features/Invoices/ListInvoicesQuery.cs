using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.Invoices
{
    public class ListInvoicesQuery : IRequest<IList<Invoice>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}