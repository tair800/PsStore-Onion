using MediatR;

namespace PsStore.Application.Features.Basket.Commands.AddToBasket
{
    public class AddToBasketCommandRequest : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }  // User who is adding the game to their basket
        public int GameId { get; set; }   // Game that is being added to the basket
    }
}
