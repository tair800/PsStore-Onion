using MediatR;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class DeleteDlcCommandRequest : IRequest<Result<Unit>>
    {
        public int Id { get; set; }
    }
}
