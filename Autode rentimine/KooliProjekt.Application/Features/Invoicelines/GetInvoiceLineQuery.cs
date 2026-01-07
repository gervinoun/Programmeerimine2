using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class GetInvoiceLineQuery : IRequest<OperationResult<object>>
    {
        public int Id { get; set; }
    }
}
