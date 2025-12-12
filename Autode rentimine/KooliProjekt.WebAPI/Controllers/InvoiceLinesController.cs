using KooliProjekt.Application.Features.InvoiceLines;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KooliProjekt.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceLinesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceLinesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var invoiceLines = await _mediator.Send(new ListInvoiceLinesQuery());
            return Ok(invoiceLines);
        }
    }
}
