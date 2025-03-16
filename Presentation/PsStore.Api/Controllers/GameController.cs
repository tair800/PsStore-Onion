using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Game.Commands;
using PsStore.Application.Features.Game.Commands.CreateGame;
using PsStore.Application.Features.Game.Queries.GetAllGame;
using PsStore.Application.Features.Game.Queries.GetGameById;

namespace PsStore.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IMediator mediator;

        public GameController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGameCommandRequest request)
        {
            await mediator.Send(request);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateGameCommandRequest request)
        {
            await mediator.Send(request);
            return Ok(new { message = "GAME updated successfully." });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await mediator.Send(new DeleteGameCommandRequest { Id = id });
            return Ok(new { message = "GAME deleted successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            await mediator.Send(new RestoreGameCommandRequest { Id = id });
            return Ok(new { message = "GAME restored successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var games = await mediator.Send(new GetAllGameQueryRequest { IncludeDeleted = includeDeleted });
            return Ok(games);
        }


        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var game = await mediator.Send(new GetGameByIdQueryRequest { Id = id });
            return Ok(game);
        }
    }
}
