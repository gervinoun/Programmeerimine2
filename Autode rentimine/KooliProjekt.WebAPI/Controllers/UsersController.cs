using KooliProjekt.Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KooliProjekt.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/Users
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await _mediator.Send(new ListUsersQuery());
            return Ok(users);
        }

        // GET api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _mediator.Send(new GetUserQuery { Id = id });
            return Ok(result);
        }

        // POST api/Users
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DELETE api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteUserCommand { Id = id });
            return Ok(result);
        }
    }
}
