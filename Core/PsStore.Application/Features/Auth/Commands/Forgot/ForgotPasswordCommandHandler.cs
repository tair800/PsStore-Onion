using MediatR;
using Microsoft.AspNetCore.Identity;
using PsStore.Application.Interfaces.Services;
using System.Net;

namespace PsStore.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommandRequest, Result<ForgotPasswordCommandResponse>>
    {
        private readonly UserManager<Domain.Entities.User> userManager;
        private readonly IEmailService emailService;

        public ForgotPasswordCommandHandler(UserManager<Domain.Entities.User> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }

        public async Task<Result<ForgotPasswordCommandResponse>> Handle(ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<ForgotPasswordCommandResponse>.Failure("User not found.", 404, "USER_NOT_FOUND");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebUtility.UrlEncode(token);

            var resetUrl = $"http://localhost:3000/reset-password?token={encodedToken}&email={request.Email}";

            var subject = "Reset Password";
            var body = $"<p>To reset your password, click the following link:</p><p><a href=\"{resetUrl}\">{resetUrl}</a></p>";
            await emailService.SendEmailAsync(request.Email, subject, body);

            return Result<ForgotPasswordCommandResponse>.Success(new ForgotPasswordCommandResponse
            {
                Message = "Password reset link has been sent to your email."
            });
        }
    }
}
