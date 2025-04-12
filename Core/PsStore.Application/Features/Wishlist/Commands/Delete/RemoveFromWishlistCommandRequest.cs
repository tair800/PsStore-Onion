using MediatR;

namespace PsStore.Application.Features.Wishlist.Commands.Remove
{
    public class RemoveFromWishlistCommandRequest : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public int? GameId { get; set; }
        public int? DlcId { get; set; }
    }
}
