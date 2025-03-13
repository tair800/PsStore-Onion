using MediatR;
using PsStore.Application.Features.Dlc.Dtos;

namespace PsStore.Application.Features.Dlc.Queries
{
    public class GetDlcByIdQuery : IRequest<DlcDto>
    {
        public int Id { get; set; }
    }
}
