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

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var carTypes = await _mediator.Send(new ListCarTypesQuery());
            return Ok(carTypes);
        }
    }
}
