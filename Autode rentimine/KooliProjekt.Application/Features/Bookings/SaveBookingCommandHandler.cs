using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Bookings
{
    public class SaveBookingCommandHandler : IRequestHandler<SaveBookingCommand, OperationResult>
    {
        private readonly IBookingRepository _repo;

        public SaveBookingCommandHandler(IBookingRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult> Handle(SaveBookingCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            Booking booking;
            if (request.Id == 0)
            {
                booking = new Booking();
                await _repo.AddAsync(booking, cancellationToken);
            }
            else
            {
                booking = await _repo.GetByIdAsync(request.Id, cancellationToken);
            }

            booking.UserId = request.UserId;
            booking.CarId = request.CarId;
            booking.StartTime = request.StartTime;
            booking.EndTime = request.EndTime;
            booking.KmStart = request.KmStart;
            booking.KmEnd = request.KmEnd;
            booking.Status = request.Status;

            await _repo.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
