using KooliProjekt.Application.Features.Bookings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KooliProjekt.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var bookings = await _mediator.Send(new ListBookingsQuery());
            return Ok(bookings);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _mediator.Send(new GetBookingQuery { Id = id });
            return Ok(result);
        }

        
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveBookingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteBookingCommand { Id = id });
            return Ok(result);
        }
    }
}
