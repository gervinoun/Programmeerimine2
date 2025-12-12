using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.Cars
{
    public class ListCarsQuery : IRequest<IList<Car>>
    {
    }
}
