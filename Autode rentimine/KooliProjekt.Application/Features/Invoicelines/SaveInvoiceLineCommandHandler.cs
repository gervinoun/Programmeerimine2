using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class SaveInvoiceLineCommandHandler : IRequestHandler<SaveInvoiceLineCommand, OperationResult>
    {
        private readonly IInvoiceLineRepository _repo;

        public SaveInvoiceLineCommandHandler(IInvoiceLineRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult> Handle(SaveInvoiceLineCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            InvoiceLine line;
            if (request.Id == 0)
            {
                line = new InvoiceLine();
                await _repo.AddAsync(line, cancellationToken);
            }
            else
            {
                line = await _repo.GetByIdAsync(request.Id, cancellationToken);
            }

            line.InvoiceId = request.InvoiceId;
            line.Description = request.Description;
            line.Amount = request.Amount;

            await _repo.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
