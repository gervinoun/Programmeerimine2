using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Features.Cars
{
    public class GetCarQueryHandler : IRequestHandler<GetCarQuery, OperationResult<object>>
    {
        private readonly ICarRepository _repo;

        public GetCarQueryHandler(ICarRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult<object>> Handle(
            GetCarQuery request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult<object>();

            if (request.Id <= 0)
            {
                return result;
            }

            result.Value = await _repo
                .Query()
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