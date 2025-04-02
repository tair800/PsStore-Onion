using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PsStore.Application.Interfaces.Services;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommandRequest, Result<Unit>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public ResetPasswordCommandHandler(
            UserManager<User> userManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<Result<Unit>> Handle(ResetPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Token);

            if (user == null)
            {
                return Result<Unit>.Failure("User not found", StatusCodes.Status404NotFound, "USER_NOT_FOUND");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                return Result<Unit>.Failure("Failed to reset password", StatusCodes.Status400BadRequest, "PASSWORD_RESET_FAILED");
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
