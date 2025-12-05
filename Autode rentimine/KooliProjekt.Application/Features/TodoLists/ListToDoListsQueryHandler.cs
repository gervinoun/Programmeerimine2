using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.TodoLists
{
    public class ListToDoListsQueryHandler : IRequestHandler<ListToDoListsQuery, OperationResult<IList<ToDoList>>>
    {
        private readonly ApplicationDbContext _dbContext;
        public ListToDoListsQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<IList<ToDoList>>> Handle(ListToDoListsQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<IList<ToDoList>>();
            result.Value = await _dbContext
                .ToDoLists
                .OrderBy(list => list.Name)
                .ToListAsync();

            return result;
        }
    }
}
