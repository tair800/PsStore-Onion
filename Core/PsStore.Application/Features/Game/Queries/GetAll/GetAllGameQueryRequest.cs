using MediatR;

namespace PsStore.Application.Features.Game.Queries.GetAllGame
{
    public class GetAllGameQueryRequest : IRequest<List<GetAllGameQueryResponse>>
    {
        public bool IncludeDeleted { get; set; } = false;
    }
}
