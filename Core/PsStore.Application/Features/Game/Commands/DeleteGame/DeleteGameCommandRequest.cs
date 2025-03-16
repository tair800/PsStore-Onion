using MediatR;

namespace PsStore.Application.Features.Game.Commands
{
    public class DeleteGameCommandRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
