using System;
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
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<OperationResult> Handle(
            SaveCarCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult();

            if (request.Id < 0)
            {
                result.AddPropertyError("Id", "Id must be zero or greater");
                return result;
            }

            Car car;

            if (request.Id == 0)
            {
                car = new Car();
                await _repo.AddAsync(car, cancellationToken);
            }
            else
            {
                car = await _repo.GetByIdAsync(
                    request.Id,
                    cancellationToken);

                if (car == null)
                {
                    result.AddError("Car not found");
                    return result;
                }
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