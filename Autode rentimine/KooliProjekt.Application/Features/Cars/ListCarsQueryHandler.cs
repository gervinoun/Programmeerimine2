using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Cars;

public class ListCarsQueryHandler : IRequestHandler<ListCarsQuery, IList<Car>>
{
    private readonly ICarRepository _repo;

    public ListCarsQueryHandler(ICarRepository repo)
    {
        _repo = repo;
    }

    public async Task<IList<Car>> Handle(ListCarsQuery request, CancellationToken cancellationToken)
    {
        return await _repo.Query().ToListAsync(cancellationToken);
    }
}
