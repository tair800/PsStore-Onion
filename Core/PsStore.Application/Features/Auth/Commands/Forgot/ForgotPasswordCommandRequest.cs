using MediatR;

namespace PsStore.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandRequest : IRequest<Result<Unit>>
    {
        public string Email { get; set; }
    }
}
