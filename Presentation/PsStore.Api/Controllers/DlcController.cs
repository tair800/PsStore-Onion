using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Dlc.Commands.CreateDlc;

namespace PsStore.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DlcController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DlcController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDlcCommandRequest request)
        {
            await _mediator.Send(request);
            return Ok(new { message = "DLC created successfully." });
        }
    }
}
