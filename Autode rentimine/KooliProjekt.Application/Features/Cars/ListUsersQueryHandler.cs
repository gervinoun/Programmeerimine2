using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Users
{
    public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, IList<User>>
    {
        private readonly ApplicationDbContext _db;

        public ListUsersQueryHandler(ApplicationDbContext db)
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
