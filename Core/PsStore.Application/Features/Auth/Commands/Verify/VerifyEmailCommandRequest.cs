using MediatR;

namespace PsStore.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandRequest : IRequest<Result<VerifyEmailCommandResponse>>
    {
        public string Email { get; set; }
    }
}
