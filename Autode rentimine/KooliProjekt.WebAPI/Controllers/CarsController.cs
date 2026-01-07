using KooliProjekt.Application.Features.Cars;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KooliProjekt.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CarsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/Cars
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var cars = await _mediator.Send(new ListCarsQuery());
            return Ok(cars);
        }

        // GET api/Cars/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _mediator.Send(new GetCarQuery { Id = id });
            return Ok(result);
        }

        // POST api/Cars
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveCarCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCarCommand { Id = id });
            return Ok(result);
        }

    }
}
