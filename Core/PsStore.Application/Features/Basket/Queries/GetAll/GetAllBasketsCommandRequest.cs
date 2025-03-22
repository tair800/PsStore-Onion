using MediatR;

namespace PsStore.Application.Features.Basket.Queries.GetAll
{
    public class GetAllBasketsCommandRequest : IRequest<Result<GetAllBasketsCommandResponse>>
    {
    }
}
