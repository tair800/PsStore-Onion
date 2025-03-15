using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Category.Commands;
using PsStore.Application.Features.Category.Commands.DeleteCategory;
using PsStore.Application.Features.Category.Commands.RestoreCategory;
using PsStore.Application.Features.Category.Commands.UpdateCategory;
using PsStore.Application.Features.Category.Queries.GetAllCategories;
using PsStore.Application.Features.Category.Queries.GetCategoriesWithGames;
using PsStore.Application.Features.Category.Queries.GetCategoryById;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await mediator.Send(new GetAllCategoriesQueryRequest());
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await mediator.Send(new GetCategoryByIdQueryRequest { Id = id });
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetWithGames()
        {
            var response = await mediator.Send(new GetCategoriesWithGamesQueryRequest());
            return Ok(response);
        }

    }
}
