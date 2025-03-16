using MediatR;

namespace PsStore.Application.Features.Game.Commands
{
    public class RestoreGameCommandRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}