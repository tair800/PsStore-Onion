using MediatR;
using PsStore.Application.Features.Dlc.Dtos;

namespace PsStore.Application.Features.Dlc.Queries
{
    public class GetAllDlcsQuery : IRequest<IList<DlcDto>>
    {
        public bool IncludeDeleted { get; set; } = false;
    }
}
