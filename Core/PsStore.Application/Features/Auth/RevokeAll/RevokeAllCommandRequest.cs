using MediatR;

namespace PsStore.Application.Features.Auth.RevokeAll
{
    public class RevokeAllCommandRequest : IRequest<Result<Unit>>
    {
    }
}
