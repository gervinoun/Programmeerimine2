using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.Users
{
    public class ListUsersQuery : IRequest<IList<User>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}