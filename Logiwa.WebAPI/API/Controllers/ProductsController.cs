using Logiwa.Business.CQRS.Commands.Products;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Logiwa.WebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("createProduct")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
