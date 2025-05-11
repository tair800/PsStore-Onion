using MediatR;

namespace PsStore.Application.Features.Auth.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandRequest : IRequest<Result<ConfirmEmailCommandResponse>>
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
