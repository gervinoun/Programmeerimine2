using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.Users
{
    public class ListBookingsQuery : IRequest<IList<Booking>>
    {
    }
}