using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Basket.Commands.AddToBasket;
using PsStore.Application.Features.Basket.Commands.Clear;
using PsStore.Application.Features.Basket.Commands.Update;
using PsStore.Application.Features.Basket.Queries.GetAll;
using PsStore.Application.Features.Basket.Queries.GetBasket;
using System.Security.Claims;

namespace PsStore.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BasketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket([FromBody] AddToBasketCommandRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdString == null || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "User not authenticated or invalid userId." });
            }

            request.UserId = userId;

            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Game added to the basket successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetBasket([FromQuery] Guid userId)
        {
            var result = await _mediator.Send(new GetBasketCommandRequest { UserId = userId });

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllBasketsCommandRequest());
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(result.Data);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFromBasket([FromBody] RemoveFromBasketCommandRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Game removed from the basket successfully." });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBasketItem([FromBody] UpdateBasketItemCommandRequest request)
        {
            var result = await _mediator.Send(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Basket item updated successfully." });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearBasket([FromQuery] Guid userId)
        {
            var result = await _mediator.Send(new ClearBasketCommandRequest { UserId = userId });
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Basket cleared successfully." });
        }
    }
}
