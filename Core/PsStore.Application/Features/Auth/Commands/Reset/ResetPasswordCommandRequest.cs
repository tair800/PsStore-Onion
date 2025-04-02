using MediatR;

namespace PsStore.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandRequest : IRequest<Result<Unit>>
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
