using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Category.Commands;

namespace PsStore.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator mediator;

        public CategoryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryCommandRequest request)
        {
            await mediator.Send(request);

            return Ok();
        }
    }
}
