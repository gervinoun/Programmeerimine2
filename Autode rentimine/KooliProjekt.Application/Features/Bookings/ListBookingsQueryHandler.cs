using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Bookings
{
    public class ListBookingsQueryHandler : IRequestHandler<ListBookingsQuery, IList<Booking>>
    {
        private readonly IBookingRepository _repo;

        public ListBookingsQueryHandler(IBookingRepository repo)
        {
            _repo = repo;
        }

        public async Task<IList<Booking>> Handle(ListBookingsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.Query().ToListAsync(cancellationToken);
        }
    }
}
