using MediatR;
using Microsoft.AspNetCore.Mvc;
using PsStore.Application.Features.Auth.Commands.ForgotPassword;
using PsStore.Application.Features.Auth.Commands.Login;
using PsStore.Application.Features.Auth.Commands.RefreshToken;
using PsStore.Application.Features.Auth.Commands.Register;
using PsStore.Application.Features.Auth.Commands.ResetPassword;
using PsStore.Application.Features.Auth.Queries.Get;
using PsStore.Application.Features.Auth.Queries.GetAll;
using PsStore.Application.Features.Auth.Revoke;
using PsStore.Application.Features.Auth.RevokeAll;
using PsStore.Application.Features.Wishlist.Commands.Clear;
using System.Security.Claims;

namespace PsStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommandRequest request)
        {
            var result = await mediator.Send(request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Error, errorCode = result.ErrorCode });
            }

            return StatusCode(StatusCodes.Status201Created, new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommandRequest request)
        {
            var result = await mediator.Send(request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Error, errorCode = result.ErrorCode });
            }

            return Ok(result.Data);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefresTokenCommandRequest request)
        {
            var result = await mediator.Send(request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Error, errorCode = result.ErrorCode });
            }

            return Ok(result.Data);
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(RevokeCommandRequest request)
        {
            var result = await mediator.Send(request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Error, errorCode = result.ErrorCode });
            }

            return Ok(new { message = "Refresh token successfully revoked." });
        }

        [HttpPost("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var result = await mediator.Send(new RevokeAllCommandRequest());
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Error, errorCode = result.ErrorCode });
            }

            return Ok(new { message = "Successfully revoked refresh tokens for all users." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommandRequest request)
        {
            var result = await mediator.Send(request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Error, errorCode = result.ErrorCode });
            }

            return Ok(new { message = result.Data.Message });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommandRequest request)
        {
            var result = await mediator.Send(request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Error, errorCode = result.ErrorCode });
            }

            return Ok(new { message = result.Data.Message });
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetUser([FromQuery] string id)
        {
            var result = await mediator.Send(new GetUserQueryRequest { Id = id });

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Error, errorCode = result.ErrorCode });
            }

            return Ok(result.Data);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await mediator.Send(new GetAllUsersQueryRequest());

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Error, errorCode = result.ErrorCode });
            }

            return Ok(result.Data);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromForm] UpdateUserCommandRequest request)
        {
            request.UserId = id;

            var result = await mediator.Send(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { message = result.Error, errorCode = result.ErrorCode });
            }

            return Ok(new { message = "User updated successfully." });
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

            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Error);
            }

            return Ok(new { message = "Wishlist cleared successfully." });
        }


    }
}
