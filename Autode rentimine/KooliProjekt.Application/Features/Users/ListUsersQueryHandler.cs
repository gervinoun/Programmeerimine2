using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Users
{
    public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, IList<User>>
    {
        private readonly IUserRepository _repo;

        public ListUsersQueryHandler(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<IList<User>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            return await _repo.Query().ToListAsync(cancellationToken);
        }
    }
}
