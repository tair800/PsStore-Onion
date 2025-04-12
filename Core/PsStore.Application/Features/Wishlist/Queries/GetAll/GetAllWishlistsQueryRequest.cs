using MediatR;

namespace PsStore.Application.Features.Wishlist.Queries.GetAll
{
    public class GetAllWishlistsQueryRequest : IRequest<Result<List<GetAllWishlistsQueryResponse>>>
    {
        public bool IncludeDeleted { get; set; } = false;
    }
}
