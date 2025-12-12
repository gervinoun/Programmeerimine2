using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.Invoices
{
    public class ListInvoicesQuery : IRequest<IList<Invoice>>
    {
    }
}
