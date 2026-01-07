using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Users
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, OperationResult<object>>
    {
        private readonly IUserRepository _repo;

        public GetUserQueryHandler(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult<object>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            result.Value = await _repo.Query()
                .Where(u => u.Id == request.Id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Phone
                    // PasswordHash intentionally not returned
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }
}
