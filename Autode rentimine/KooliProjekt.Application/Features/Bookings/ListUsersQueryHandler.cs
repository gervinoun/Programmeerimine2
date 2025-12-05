using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Users
{
    public class ListBookingsQueryHandler : IRequestHandler<ListBookingsQuery, IList<Booking>>
    {
        private readonly ApplicationDbContext _db;

        public ListBookingsQueryHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IList<User>> Handle(
            ListUsersQuery request,
            CancellationToken cancellationToken)
        {
            return await _db.Users.ToListAsync(cancellationToken);
        }
    }
}
