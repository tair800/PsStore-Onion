using MediatR;

namespace PsStore.Application.Features.Wishlist.Queries.GetWishlistByUserId
{
    public class GetWishlistByUserIdQueryRequest : IRequest<Result<GetWishlistByUserIdQueryResponse>>
    {
        public Guid UserId { get; set; }
    }
}
