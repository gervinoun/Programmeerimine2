using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class ListInvoiceLinesQuery : IRequest<IList<InvoiceLine>>
    {
    }
}
