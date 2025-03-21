using MediatR;

namespace PsStore.Application.Features.Dlc.Queries.GetAllDlc
{
    public class GetAllDlcQueryRequest : IRequest<Result<List<GetAllDlcQueryResponse>>>
    {
        public bool IncludeDeleted { get; set; } = false;
    }
}
