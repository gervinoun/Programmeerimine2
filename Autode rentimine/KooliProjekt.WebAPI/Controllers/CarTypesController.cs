using KooliProjekt.Application.Features.CarTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KooliProjekt.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CarTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/CarTypes
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var carTypes = await _mediator.Send(new ListCarTypesQuery());
            return Ok(carTypes);
        }

        // GET api/CarTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _mediator.Send(new GetCarTypeQuery { Id = id });
            return Ok(result);
        }

        // POST api/CarTypes
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveCarTypeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DELETE api/CarTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCarTypeCommand { Id = id });
            return Ok(result);
        }
    }
}
