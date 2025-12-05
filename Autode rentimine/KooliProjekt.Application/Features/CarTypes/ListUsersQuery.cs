using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.Users
{
    public class ListUsersQuery : IRequest<IList<User>>
    {
    }
}
