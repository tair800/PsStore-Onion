using MediatR;

namespace PsStore.Application.Features.Game.Queries.GetAllGame
{
    public class GetAllGameQueryRequest : IRequest<Result<List<GetAllGameQueryResponse>>>
    {
        public bool IncludeDeleted { get; set; } = false;
    }
}
