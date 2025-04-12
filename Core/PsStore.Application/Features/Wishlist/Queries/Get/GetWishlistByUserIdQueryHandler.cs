using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Wishlist.Queries.GetWishlistByUserId
{
    public class GetWishlistByUserIdQueryHandler : IRequestHandler<GetWishlistByUserIdQueryRequest, Result<GetWishlistByUserIdQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetWishlistByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetWishlistByUserIdQueryResponse>> Handle(GetWishlistByUserIdQueryRequest request, CancellationToken cancellationToken)
        {
            var wishlist = await _unitOfWork.GetReadRepository<Domain.Entities.Wishlist>().GetAsync(
                w => w.UserId == request.UserId,
                include: query => query
                    .Include(w => w.WishlistItems)
                        .ThenInclude(wi => wi.Game)
                    .Include(w => w.WishlistItems)
                        .ThenInclude(wi => wi.Dlc),
                enableTracking: false
            );

            if (wishlist == null)
            {
                return Result<GetWishlistByUserIdQueryResponse>.Failure("Wishlist not found.", StatusCodes.Status404NotFound, "WISHLIST_NOT_FOUND");
            }

            var response = new GetWishlistByUserIdQueryResponse
            {
                WishlistId = wishlist.Id,
                Items = wishlist.WishlistItems.Select(item => new WishlistItem
                {
                    GameId = item.GameId,
                    GameTitle = item.Game?.Title,
                    DlcId = item.DlcId,
                    DlcName = item.Dlc?.Name
                }).ToList()
            };

            return Result<GetWishlistByUserIdQueryResponse>.Success(response);
        }
    }
}
