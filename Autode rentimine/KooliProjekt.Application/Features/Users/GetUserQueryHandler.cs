using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Features.Users
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, OperationResult<object>>
    {
        private readonly IUserRepository _repo;

        public GetUserQueryHandler(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult<object>> Handle(
            GetUserQuery request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult<object>();

            if (request.Id <= 0)
            {
                return result;
            }

            result.Value = await _repo.Query()
                .Where(u => u.Id == request.Id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Phone
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }
}