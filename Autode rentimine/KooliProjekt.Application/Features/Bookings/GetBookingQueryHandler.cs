using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Bookings
{
    public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, OperationResult<object>>
    {
        private readonly IBookingRepository _repo;

        public GetBookingQueryHandler(IBookingRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult<object>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            result.Value = await _repo.Query()
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
