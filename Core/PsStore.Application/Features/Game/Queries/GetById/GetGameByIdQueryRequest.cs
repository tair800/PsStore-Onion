using MediatR;

namespace PsStore.Application.Features.Game.Queries.GetGameById
{
    public class GetGameByIdQueryRequest : IRequest<GetGameByIdQueryResponse>
    {
        public int Id { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
