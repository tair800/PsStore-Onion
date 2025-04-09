using MediatR;
using PsStore.Application.Features.Dlc.Queries.GetDlcById;

namespace PsStore.Application.Features.Game.Queries.GetDlcByGame
{
    public class GetDlcsByGameIdQueryRequest : IRequest<Result<List<GetDlcByIdQueryResponse>>>
    {
        public int GameId { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }

}
