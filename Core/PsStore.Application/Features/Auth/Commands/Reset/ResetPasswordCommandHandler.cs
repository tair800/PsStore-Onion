using MediatR;
using Microsoft.AspNetCore.Identity;
using PsStore.Application.Features.Auth.Commands.ResetPassword;
using PsStore.Domain.Entities;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommandRequest, Result<ResetPasswordCommandResponse>>
{
    private readonly UserManager<User> userManager;

    public ResetPasswordCommandHandler(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<Result<ResetPasswordCommandResponse>> Handle(ResetPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<ResetPasswordCommandResponse>.Failure("User not found.", 404, "USER_NOT_FOUND");
        }

        var resetResult = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!resetResult.Succeeded)
        {
            return Result<ResetPasswordCommandResponse>.Failure("Invalid token or password reset failed.", 400, "RESET_PASSWORD_FAILED");
        }

        return Result<ResetPasswordCommandResponse>.Success(new ResetPasswordCommandResponse
        {
            Message = "Password has been successfully reset."
        });
    }
}
