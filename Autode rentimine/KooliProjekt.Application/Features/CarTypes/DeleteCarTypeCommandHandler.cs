using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.CarTypes
{
    public class DeleteCarTypeCommandHandler : IRequestHandler<DeleteCarTypeCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteCarTypeCommandHandler(ApplicationDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(
            DeleteCarTypeCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult();

            if (request.Id <= 0)
            {
                return result;
            }

            var carType = await _dbContext.CarTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (carType == null)
            {
                return result;
            }

            _dbContext.CarTypes.Remove(carType);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}