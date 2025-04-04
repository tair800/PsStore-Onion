using MediatR;

namespace PsStore.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandRequest : IRequest<Result<ResetPasswordCommandResponse>>
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
