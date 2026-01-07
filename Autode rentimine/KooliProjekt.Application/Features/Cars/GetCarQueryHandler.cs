using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Cars
{
    public class GetCarQueryHandler : IRequestHandler<GetCarQuery, OperationResult<object>>
    {
        private readonly ICarRepository _repo;

        public GetCarQueryHandler(ICarRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult<object>> Handle(GetCarQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            result.Value = await _repo.Query()
                .Where(c => c.Id == request.Id)
                .Select(c => new
                {
                    c.Id,
                    c.NumberPlate,
                    c.TypeId,
                    c.Kmrate,
                    c.TimeRate,
                    c.IsAvailable
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }
}
