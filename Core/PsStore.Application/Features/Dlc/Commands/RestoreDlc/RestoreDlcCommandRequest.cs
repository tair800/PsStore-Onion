using MediatR;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class RestoreDlcCommandRequest : IRequest<Result<Unit>>
    {
        public int Id { get; set; }
    }
}
