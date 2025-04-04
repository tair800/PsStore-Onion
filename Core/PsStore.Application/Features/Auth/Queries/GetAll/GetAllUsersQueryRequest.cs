using MediatR;

namespace PsStore.Application.Features.Auth.Queries.GetAll
{
    public class GetAllUsersQueryRequest : IRequest<Result<List<GetAllUsersQueryResponse>>>
    {
    }
}
