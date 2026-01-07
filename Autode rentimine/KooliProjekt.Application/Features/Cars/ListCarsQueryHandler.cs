using KooliProjekt.Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Features.Cars;

public class ListCarsQueryHandler : IRequestHandler<ListCarsQuery, IList<Car>>
{
    private readonly ApplicationDbContext _db;

    public ListCarsQueryHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IList<Car>> Handle(ListCarsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Cars.ToListAsync(cancellationToken);
    }
}
