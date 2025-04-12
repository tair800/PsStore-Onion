using MediatR;

namespace PsStore.Application.Features.Basket.Queries.GetBasket
{
    public class GetBasketCommandRequest : IRequest<Result<GetBasketCommandResponse>>
    {
        public Guid UserId { get; set; }
    }
}
