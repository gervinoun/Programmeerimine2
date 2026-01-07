using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Users
{
    public class SaveUserCommandHandler : IRequestHandler<SaveUserCommand, OperationResult>
    {
        private readonly IUserRepository _repo;

        public SaveUserCommandHandler(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<OperationResult> Handle(SaveUserCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            User user;
            if (request.Id == 0)
            {
                user = new User();
                await _repo.AddAsync(user, cancellationToken);
            }
            else
            {
                user = await _repo.GetByIdAsync(request.Id, cancellationToken);
            }

            user.Name = request.Name;
            user.Email = request.Email;
            user.PasswordHash = request.PasswordHash;
            user.Phone = request.Phone;

            await _repo.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
