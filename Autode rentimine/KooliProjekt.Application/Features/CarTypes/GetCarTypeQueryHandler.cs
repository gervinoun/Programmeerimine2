using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Features.CarTypes
{
    public class GetCarTypeQueryHandler : IRequestHandler<GetCarTypeQuery, OperationResult<object>>
    {
        private readonly ICarTypeRepository _repo;

        public GetCarTypeQueryHandler(ICarTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult<object>> Handle(
            GetCarTypeQuery request,
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

            result.Value = await _repo.Query()
                .Where(x => x.Id == request.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }
}