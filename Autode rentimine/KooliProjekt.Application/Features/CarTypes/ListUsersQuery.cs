using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.CarTypes
{
    public class ListCarTypesQuery : IRequest<IList<CarType>>
    {
    }
}
