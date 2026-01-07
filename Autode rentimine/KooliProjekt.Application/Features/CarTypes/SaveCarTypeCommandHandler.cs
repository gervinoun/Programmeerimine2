using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.CarTypes
{
    public class SaveCarTypeCommandHandler : IRequestHandler<SaveCarTypeCommand, OperationResult>
    {
        private readonly ICarTypeRepository _repo;

        public SaveCarTypeCommandHandler(ICarTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult> Handle(SaveCarTypeCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            CarType entity;
            if (request.Id == 0)
            {
                entity = new CarType();
                await _repo.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity = await _repo.GetByIdAsync(request.Id, cancellationToken);
            }

            entity.Name = request.Name;

            await _repo.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
