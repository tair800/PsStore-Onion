using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Game.Commands;
using PsStore.Application.Features.Game.Commands.CreateGame;
using PsStore.Application.Features.Game.Queries.GetAllGame;
using PsStore.Application.Features.Game.Queries.GetGameById;

namespace PsStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateGameCommandRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return StatusCode(StatusCodes.Status201Created, "Game created successfully.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateGameCommandRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Game updated successfully." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteGameCommandRequest { Id = id });

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Game deleted successfully." });
        }

        [HttpPost("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _mediator.Send(new RestoreGameCommandRequest { Id = id });

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Game restored successfully." });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var result = await _mediator.Send(new GetAllGameQueryRequest { IncludeDeleted = includeDeleted });

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromQuery] bool includeDeleted = false)
        {
            var result = await _mediator.Send(new GetGameByIdQueryRequest
            {
                Id = id,
                IncludeDeleted = includeDeleted
            });

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return Ok(result.Data);
        }

    }
}
