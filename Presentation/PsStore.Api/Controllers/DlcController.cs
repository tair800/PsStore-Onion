﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Dlc.Commands;
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
            var result = await _mediator.Send(request);
            return CreatedAtAction(nameof(Create), new { id = result }, new { message = "DLC created successfully." });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteDlcCommandRequest { Id = id });
            return Ok(new { message = "DLC deleted successfully." });
        }

        [HttpPost("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            await _mediator.Send(new RestoreDlcCommandRequest { Id = id });
            return Ok(new { message = "DLC restored successfully." });
        }
    }
}
