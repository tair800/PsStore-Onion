using MediatR;

namespace PsStore.Application.Features.Basket.Commands.Update
{
    public class UpdateBasketItemCommandRequest : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public int GameId { get; set; }
        public double Price { get; set; }
    }

}
