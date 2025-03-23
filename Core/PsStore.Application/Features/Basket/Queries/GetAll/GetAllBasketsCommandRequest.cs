using MediatR;

namespace PsStore.Application.Features.Basket.Queries.GetAllBaskets
{
    public class GetAllBasketsCommandRequest : IRequest<Result<GetAllBasketsCommandResponse>>
    {
        // You can add any filtering or pagination parameters here if needed
    }
}
