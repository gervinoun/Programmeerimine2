using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.CarTypes
{
    public class ListCarTypesQueryHandler : IRequestHandler<ListCarTypesQuery, IList<CarType>>
    {
        private readonly ICarTypeRepository _repo;

        public ListCarTypesQueryHandler(ICarTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<IList<CarType>> Handle(ListCarTypesQuery request, CancellationToken cancellationToken)
        {
            return await _repo.Query().ToListAsync(cancellationToken);
        }
    }
}
