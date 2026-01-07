using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Bookings
{

    public class DeleteBookingCommand : IRequest<OperationResult>
    {
        public int Id { get; set; }
    }
}
