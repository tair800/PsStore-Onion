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
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommandRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return StatusCode(StatusCodes.Status201Created, new { message = "CATEGORY created successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> Restore([FromBody] RestoreCategoryCommandRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "CATEGORY restored successfully." });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryCommandRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "CATEGORY updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommandRequest { Id = id });

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Category deleted successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var result = await _mediator.Send(new GetAllCategoriesQueryRequest { IncludeDeleted = includeDeleted });

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(result.Data);
        }


        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQueryRequest { Id = id });

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesWithGames()
        {
            var result = await _mediator.Send(new GetCategoriesWithGamesQueryRequest());

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(result.Data);
        }

    }
}
