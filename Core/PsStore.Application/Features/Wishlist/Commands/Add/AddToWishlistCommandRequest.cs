using MediatR;

namespace PsStore.Application.Features.Wishlist.Commands.Add
{
    public class AddToWishlistCommandRequest : IRequest<Result<Unit>>
    {
        public Guid UserId { get; set; }
        public int? GameId { get; set; }
        public int? DlcId { get; set; }
    }
}
