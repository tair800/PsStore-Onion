using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Game.Commands;
using PsStore.Application.Features.Game.Commands.CreateGame;

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
    }
}
