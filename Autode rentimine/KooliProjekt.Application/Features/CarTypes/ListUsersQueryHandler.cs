using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.CarTypes
{
    public class ListCarTypesQueryHandler
        : IRequestHandler<ListCarTypesQuery, IList<CarType>>
    {
        private readonly ApplicationDbContext _db;

        public ListCarTypesQueryHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IList<CarType>> Handle(
            ListCarTypesQuery request,
            CancellationToken cancellationToken)
        {
            return await _db.CarTypes.ToListAsync(cancellationToken);
        }
    }
}
