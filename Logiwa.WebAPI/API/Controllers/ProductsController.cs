using Logiwa.Business.CQRS.Commands.Products;
using Logiwa.Business.CQRS.Queries.Products;
using Logiwa.Business.DTOs.Products.Outputs;
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

        [HttpPost("updateProduct")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }


        [HttpDelete("deleteProduct")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteProduct([FromBody] DeleteProductCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("updateStockQuantity")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProductOutputDTO))]
        public async Task<IActionResult> UpdateStockQuantity([FromBody] UpdateStockQuantityCommand command, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(command, cancellationToken));
        }

        [HttpGet("getProductByStockCode")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProductOutputDTO))]
        public async Task<IActionResult> GetProductByStockCode([FromQuery] string stockCode, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetProductByStockCode(stockCode), cancellationToken));
        }
    }
}
