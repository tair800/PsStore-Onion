using MediatR;

namespace PsStore.Application.Features.Game.Queries.GetGameById
{
    public class GetGameByIdQueryRequest : IRequest<Result<GetGameByIdQueryResponse>>
    {
        public int Id { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
