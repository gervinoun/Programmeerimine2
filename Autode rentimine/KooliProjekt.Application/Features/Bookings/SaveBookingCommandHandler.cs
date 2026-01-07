using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Bookings
{
    public class SaveBookingCommandHandler : IRequestHandler<SaveBookingCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public SaveBookingCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(SaveBookingCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            var booking = new Booking();
            if (request.Id == 0)
            {
                await _dbContext.Bookings.AddAsync(booking, cancellationToken);
            }
            else
            {
                booking = await _dbContext.Bookings.FindAsync(new object[] { request.Id }, cancellationToken);
                // teacher-style: no error handling; optional:
                // if (booking == null) return result;
            }

            booking.UserId = request.UserId;
            booking.CarId = request.CarId;
            booking.StartTime = request.StartTime;
            booking.EndTime = request.EndTime;
            booking.KmStart = request.KmStart;
            booking.KmEnd = request.KmEnd;
            booking.Status = request.Status;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
