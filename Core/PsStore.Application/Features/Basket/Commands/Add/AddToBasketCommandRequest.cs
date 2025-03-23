using MediatR;

namespace PsStore.Application.Features.Basket.Commands.AddToBasket
{
    public class AddToBasketCommandRequest : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public int? GameId { get; set; }
        public int? DlcId { get; set; }
    }
}
