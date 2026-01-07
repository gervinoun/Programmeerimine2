using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.CarTypes
{
    public class SaveCarTypeCommandHandler : IRequestHandler<SaveCarTypeCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public SaveCarTypeCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(SaveCarTypeCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            var entity = new CarType();
            if (request.Id == 0)
            {
                await _dbContext.CarTypes.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity = await _dbContext.CarTypes.FindAsync(new object[] { request.Id }, cancellationToken);
                // teacher-style: no explicit error handling
            }

            entity.Name = request.Name;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
