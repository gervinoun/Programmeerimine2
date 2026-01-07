using KooliProjekt.Application.Features.Invoices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KooliProjekt.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/Invoices
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var invoices = await _mediator.Send(new ListInvoicesQuery());
            return Ok(invoices);
        }

        // GET api/Invoices/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _mediator.Send(new GetInvoiceQuery { Id = id });
            return Ok(result);
        }

        // POST api/Invoices
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveInvoiceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DELETE api/Invoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteInvoiceCommand { Id = id });
            return Ok(result);
        }
    }
}
