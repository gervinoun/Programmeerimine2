using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Bookings
{

    public class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteBookingCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            await _dbContext
                .Bookings
                .Where(x => x.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            return result;
        }
    }
}
