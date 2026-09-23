using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Cars
{
    public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteCarCommandHandler(ApplicationDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(
            DeleteCarCommand request,
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

            var car = await _dbContext.Cars
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (car == null)
            {
                return result;
            }

            _dbContext.Cars.Remove(car);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}