using MediatR;

namespace PsStore.Application.Features.Wishlist.Commands.Clear
{
    public class ClearWishlistCommandRequest : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
    }
}
