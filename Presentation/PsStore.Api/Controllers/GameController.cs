using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Game.Commands;

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
    }
}
