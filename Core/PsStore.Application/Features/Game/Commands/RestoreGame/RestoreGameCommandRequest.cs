using MediatR;

namespace PsStore.Application.Features.Game.Commands
{
    public class RestoreGameCommandRequest : IRequest<Result<Unit>>
    {
        public int Id { get; set; }
    }
}