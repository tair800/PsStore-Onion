using MediatR;

namespace PsStore.Application.Features.Basket.Commands.Remove
{
    public class RemoveFromBasketCommandRequest : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public int? GameId { get; set; }
        public int? DlcId { get; set; }
    }
}
