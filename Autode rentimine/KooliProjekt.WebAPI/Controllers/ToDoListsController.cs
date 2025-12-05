using System.Threading.Tasks;
using KooliProjekt.Application.Features.TodoLists;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KooliProjekt.WebAPI.Controllers
{
    public class ToDoListsController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public ToDoListsController(IMediator mediator) 
        { 
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var query = new ListToDoListsQuery();
            var result = await _mediator.Send(query);

            return Result(result);
        }
    }
}
