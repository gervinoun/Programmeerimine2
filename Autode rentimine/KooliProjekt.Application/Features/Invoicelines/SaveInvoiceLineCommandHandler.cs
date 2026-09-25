using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.InvoiceLines
{
    public class SaveInvoiceLineCommandHandler
        : IRequestHandler<SaveInvoiceLineCommand, OperationResult>
    {
        private readonly IInvoiceLineRepository _repo;

        public SaveInvoiceLineCommandHandler(IInvoiceLineRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<OperationResult> Handle(
            SaveInvoiceLineCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult();

            if (request.Id < 0)
            {
                result.AddPropertyError(
                    "Id",
                    "Id must be zero or greater");

                return result;
            }

            InvoiceLine invoiceLine;

            if (request.Id == 0)
            {
                invoiceLine = new InvoiceLine();

                await _repo.AddAsync(
                    invoiceLine,
                    cancellationToken);
            }
            else
            {
                invoiceLine = await _repo.GetByIdAsync(
                    request.Id,
                    cancellationToken);

                if (invoiceLine == null)
                {
                    result.AddError("Invoice line not found");
                    return result;
                }
            }

            invoiceLine.InvoiceId = request.InvoiceId;
            invoiceLine.Description = request.Description;
            invoiceLine.Amount = request.Amount;

            await _repo.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}