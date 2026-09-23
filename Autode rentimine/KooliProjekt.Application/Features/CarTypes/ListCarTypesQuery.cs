using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.CarTypes
{
    public class ListCarTypesQuery : IRequest<IList<CarType>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}