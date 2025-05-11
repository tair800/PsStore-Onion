using MediatR;
using Microsoft.AspNetCore.Identity;

namespace PsStore.Application.Features.Auth.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommandRequest, Result<ConfirmEmailCommandResponse>>
    {
        private readonly UserManager<Domain.Entities.User> userManager;

        public ConfirmEmailCommandHandler(UserManager<Domain.Entities.User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<Result<ConfirmEmailCommandResponse>> Handle(ConfirmEmailCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Result<ConfirmEmailCommandResponse>.Failure("User not found.", 404, "USER_NOT_FOUND");

            if (user.EmailConfirmed)
                return Result<ConfirmEmailCommandResponse>.Failure("Email is already verified.", 400, "EMAIL_ALREADY_CONFIRMED");

            var result = await userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
                return Result<ConfirmEmailCommandResponse>.Failure("Invalid or expired token.", 400, "EMAIL_CONFIRMATION_FAILED");

            return Result<ConfirmEmailCommandResponse>.Success(new ConfirmEmailCommandResponse
            {
                Message = "Email confirmed successfully."
            });
        }
    }
}
