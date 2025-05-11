using MediatR;
using Microsoft.AspNetCore.Identity;
using PsStore.Application.Interfaces.Services;
using System.Net;

namespace PsStore.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommandRequest, Result<VerifyEmailCommandResponse>>
    {
        private readonly UserManager<Domain.Entities.User> userManager;
        private readonly IEmailService emailService;

        public VerifyEmailCommandHandler(UserManager<Domain.Entities.User> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }

        public async Task<Result<VerifyEmailCommandResponse>> Handle(VerifyEmailCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<VerifyEmailCommandResponse>.Failure("User not found.", 404, "USER_NOT_FOUND");
            }

            if (user.EmailConfirmed)
            {
                return Result<VerifyEmailCommandResponse>.Failure("Email is already verified.", 400, "EMAIL_ALREADY_CONFIRMED");
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            var confirmUrl = $"http://localhost:5000/api/auth/confirm-email?token={encodedToken}&email={user.Email}";

            var subject = "Confirm Your Email";
            var body = $"<p>Click the link below to confirm your email:</p><p><a href=\"{confirmUrl}\">{confirmUrl}</a></p>";

            await emailService.SendEmailAsync(request.Email, subject, body);

            return Result<VerifyEmailCommandResponse>.Success(new VerifyEmailCommandResponse
            {
                Message = "Verification link has been sent to your email."
            });
        }
    }
}
