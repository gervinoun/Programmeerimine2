using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Cars
{
    public class GetCarQueryHandler : IRequestHandler<GetCarQuery, OperationResult<object>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetCarQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<object>> Handle(GetCarQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            result.Value = await _dbContext
                .Cars
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
