using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Cars
{
    public class SaveCarCommandHandler : IRequestHandler<SaveCarCommand, OperationResult>
    {
        private readonly ICarRepository _repo;

        public SaveCarCommandHandler(ICarRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult> Handle(SaveCarCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            Car car;
            if (request.Id == 0)
            {
                car = new Car();
                await _repo.AddAsync(car, cancellationToken);
            }
            else
            {
                car = await _repo.GetByIdAsync(request.Id, cancellationToken);
            }

            car.NumberPlate = request.NumberPlate;
            car.TypeId = request.TypeId;
            car.Kmrate = request.Kmrate;
            car.TimeRate = request.TimeRate;
            car.IsAvailable = request.IsAvailable;

            await _repo.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
