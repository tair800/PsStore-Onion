using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PsStore.Application.Interfaces.Services;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommandRequest, Result<Unit>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public ForgotPasswordCommandHandler(
            UserManager<User> userManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<Result<Unit>> Handle(ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<Unit>.Failure("User not found", StatusCodes.Status404NotFound, "USER_NOT_FOUND");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{_configuration["AppUrl"]}/reset-password?token={token}";

            var emailSubject = "Password Reset Request";
            var emailBody = $"<p>Click the following link to reset your password:</p><a href='{resetLink}'>Reset Password</a>";

            var emailSent = await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            if (!emailSent)
            {
                return Result<Unit>.Failure("Failed to send email", StatusCodes.Status500InternalServerError, "EMAIL_SEND_FAILED");
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
