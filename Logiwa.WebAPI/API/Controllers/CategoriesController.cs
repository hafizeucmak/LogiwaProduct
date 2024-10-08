using Logiwa.Business.CQRS.Commands.Categories;
using Logiwa.Business.DTOs.Categories.Outputs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Logiwa.WebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("createCategory")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CategoryOutputDTO))]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {            
            return Ok(await _mediator.Send(command, cancellationToken));
        }       
    }
}
