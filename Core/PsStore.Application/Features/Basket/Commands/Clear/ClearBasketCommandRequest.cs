using MediatR;

namespace PsStore.Application.Features.Basket.Commands.Clear
{
    public class ClearBasketCommandRequest : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
    }

}
