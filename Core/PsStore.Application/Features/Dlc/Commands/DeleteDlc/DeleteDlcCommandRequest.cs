using MediatR;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class DeleteDlcCommandRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
