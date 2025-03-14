using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Category.Commands;
using PsStore.Application.Features.Category.Commands.DeleteCategory;
using PsStore.Application.Features.Category.Commands.RestoreCategory;
using PsStore.Application.Features.Category.Commands.UpdateCategory;

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
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommandRequest request)
        {
            await mediator.Send(request);
            return Ok(new { message = "CATEGORY created successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> Restore([FromBody] RestoreCategoryCommandRequest request)
        {
            await mediator.Send(request);
            return Ok(new { message = "CATEGORY restored successfully." });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryCommandRequest request)
        {
            await mediator.Send(request);
            return Ok(new { message = "CATEGORY updated successfully." });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteCategoryCommandRequest request)
        {
            await mediator.Send(request);
            return Ok(new { message = "Category deleted successfully." });
        }
    }
}
