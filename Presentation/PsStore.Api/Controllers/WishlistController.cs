using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Wishlist.Commands.Add;
using PsStore.Application.Features.Wishlist.Commands.Clear;
using PsStore.Application.Features.Wishlist.Commands.Remove;
using PsStore.Application.Features.Wishlist.Queries.GetAll;
using PsStore.Application.Features.Wishlist.Queries.GetWishlistByUserId;
using System.Security.Claims;

namespace PsStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WishlistController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistCommandRequest request)
        {
            if (request.GameId == 0)
            {
                request.GameId = null;
            }

            if (request.DlcId == 0)
            {
                request.DlcId = null;
            }

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

            return Ok(new { message = "Game or DLC added to the wishlist successfully." });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishlistByUserId(Guid userId)
        {
            var query = new GetWishlistByUserIdQueryRequest
            {
                UserId = userId
            };

            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(result.Data);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllWishlists()
        {
            var query = new GetAllWishlistsQueryRequest();
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(result.Data);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFromWishlist([FromQuery] int? gameId, [FromQuery] int? dlcId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdString == null || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "User not authenticated or invalid userId." });
            }

            if (!gameId.HasValue && !dlcId.HasValue)
            {
                return BadRequest(new { message = "Either gameId or dlcId must be provided." });
            }

            var command = new RemoveFromWishlistCommandRequest
            {
                UserId = userId,
                GameId = gameId,
                DlcId = dlcId
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Item removed from wishlist successfully." });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdString == null || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "User not authenticated or invalid userId." });
            }

            var command = new ClearWishlistCommandRequest
            {
                UserId = userId
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Wishlist cleared successfully." });
        }



    }
}
