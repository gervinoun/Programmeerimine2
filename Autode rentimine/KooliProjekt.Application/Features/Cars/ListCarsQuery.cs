using System.Collections.Generic;
using KooliProjekt.Application.Data;
using MediatR;

namespace KooliProjekt.Application.Features.Cars
{
    public class ListCarsQuery : IRequest<IList<Car>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}