using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Cars
{
    public class SaveCarCommandHandler : IRequestHandler<SaveCarCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public SaveCarCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(SaveCarCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            var car = new Car();
            if (request.Id == 0)
            {
                await _dbContext.Cars.AddAsync(car, cancellationToken);
            }
            else
            {
                car = await _dbContext.Cars.FindAsync(new object[] { request.Id }, cancellationToken);
                // optional: if (car == null) return result;
            }

            car.NumberPlate = request.NumberPlate;
            car.TypeId = request.TypeId;
            car.Kmrate = request.Kmrate;
            car.TimeRate = request.TimeRate;
            car.IsAvailable = request.IsAvailable;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
