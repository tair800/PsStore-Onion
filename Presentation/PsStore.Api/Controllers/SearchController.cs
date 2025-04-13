using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Search.Queries;

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        var result = await _mediator.Send(new SearchProductsQueryRequest { Keyword = keyword });

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, result.Error);

        return Ok(result.Data);
    }
}
