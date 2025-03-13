using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Dlc.Commands;
using PsStore.Application.Features.Dlc.Commands.CreateDlc;
using PsStore.Application.Features.Dlc.Queries.GetAllDlc;
using PsStore.Application.Features.Dlc.Queries.GetDlcById;

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
            var result = await _mediator.Send(request);
            return CreatedAtAction(nameof(Create), new { id = result }, new { message = "DLC created successfully." });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDlcCommandRequest request)
        {
            await _mediator.Send(request);
            return Ok(new { message = "DLC updated successfully." });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteDlcCommandRequest { Id = id });
            return Ok(new { message = "DLC deleted successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            await _mediator.Send(new RestoreDlcCommandRequest { Id = id });
            return Ok(new { message = "DLC restored successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var dlcs = await _mediator.Send(new GetAllDlcQueryRequest { IncludeDeleted = includeDeleted });
            return Ok(dlcs);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id, [FromQuery] bool includeDeleted = false)
        {
            var dlc = await _mediator.Send(new GetDlcByIdQueryRequest { Id = id, IncludeDeleted = includeDeleted });
            return Ok(dlc);
        }

    }
}
