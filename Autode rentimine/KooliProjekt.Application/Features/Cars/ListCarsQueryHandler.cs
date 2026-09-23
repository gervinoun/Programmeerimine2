using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Features.Cars
{
    public class ListCarsQueryHandler : IRequestHandler<ListCarsQuery, IList<Car>>
    {
        private const int MaxPageSize = 100;

        private readonly ICarRepository _repo;

        public ListCarsQueryHandler(ICarRepository repo)
        {
            _repo = repo;
        }

        public async Task<IList<Car>> Handle(
            ListCarsQuery request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Page <= 0)
            {
                throw new ArgumentException(
                    "Page peab olema suurem kui 0.",
                    nameof(request.Page));
            }

            if (request.PageSize <= 0)
            {
                throw new ArgumentException(
                    "PageSize peab olema suurem kui 0.",
                    nameof(request.PageSize));
            }

            if (request.PageSize > MaxPageSize)
            {
                throw new ArgumentException(
                    $"PageSize ei tohi olla suurem kui {MaxPageSize}.",
                    nameof(request.PageSize));
            }

            return await _repo
                .Query()
                .OrderBy(c => c.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }
    }
}