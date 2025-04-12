using MediatR;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Wishlist.Queries.GetAll
{
    public class GetAllWishlistsQueryHandler : IRequestHandler<GetAllWishlistsQueryRequest, Result<List<GetAllWishlistsQueryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllWishlistsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<GetAllWishlistsQueryResponse>>> Handle(GetAllWishlistsQueryRequest request, CancellationToken cancellationToken)
        {
            var wishlists = await _unitOfWork
                .GetReadRepository<Domain.Entities.Wishlist>()
                .GetAllAsync(
                    include: query => query
                        .Include(w => w.WishlistItems)
                            .ThenInclude(wi => wi.Game)
                        .Include(w => w.WishlistItems)
                            .ThenInclude(wi => wi.Dlc),
                    enableTracking: false,
                    includeDeleted: request.IncludeDeleted
                );

            var response = wishlists.Select(w => new GetAllWishlistsQueryResponse
            {
                WishlistId = w.Id,
                UserId = w.UserId,
                Items = w.WishlistItems.Select(item => new WishlistItems
                {
                    GameId = item.GameId,
                    GameTitle = item.Game?.Title,
                    DlcId = item.DlcId,
                    DlcName = item.Dlc?.Name
                }).ToList()
            }).ToList();

            return Result<List<GetAllWishlistsQueryResponse>>.Success(response);
        }
    }
}
