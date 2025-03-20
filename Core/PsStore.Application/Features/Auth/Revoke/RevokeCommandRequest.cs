using MediatR;

namespace PsStore.Application.Features.Auth.Revoke
{
    public class RevokeCommandRequest : IRequest<Result<Unit>>
    {
        public string Email { get; set; }
    }
}
