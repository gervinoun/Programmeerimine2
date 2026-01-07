using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Bookings
{
    public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, OperationResult<object>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetBookingQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<object>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            result.Value = await _dbContext
                .Bookings
                .Where(b => b.Id == request.Id)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    b.CarId,
                    b.StartTime,
                    b.EndTime,
                    b.KmStart,
                    b.KmEnd,
                    b.Status
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }
}
