using System;
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
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(
            DeleteBookingCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult();

            if (request.Id <= 0)
            {
                return result;
            }

            var booking = await _dbContext.Bookings
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (booking == null)
            {
                return result;
            }

            _dbContext.Bookings.Remove(booking);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}