using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Wishlist.Commands.Remove
{
    public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveFromWishlistCommandHandler> _logger;

        public RemoveFromWishlistCommandHandler(IUnitOfWork unitOfWork, ILogger<RemoveFromWishlistCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(RemoveFromWishlistCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var wishlist = await _unitOfWork.GetReadRepository<Domain.Entities.Wishlist>()
                    .GetAsync(w => w.UserId == request.UserId);

                if (wishlist == null)
                {
                    _logger.LogWarning("Wishlist not found for user {UserId}.", request.UserId);
                    return Result<Unit>.Failure("Wishlist not found.", StatusCodes.Status404NotFound, "WISHLIST_NOT_FOUND");
                }

                WishlistItem wishlistItem = null;

                if (request.GameId.HasValue)
                {
                    wishlistItem = await _unitOfWork.GetReadRepository<WishlistItem>()
                        .GetAsync(wi => wi.WishlistId == wishlist.Id && wi.GameId == request.GameId);
                }
                else if (request.DlcId.HasValue)
                {
                    wishlistItem = await _unitOfWork.GetReadRepository<WishlistItem>()
                        .GetAsync(wi => wi.WishlistId == wishlist.Id && wi.DlcId == request.DlcId);
                }

                if (wishlistItem == null)
                {
                    return Result<Unit>.Failure("Item not found in wishlist.", StatusCodes.Status404NotFound, "ITEM_NOT_FOUND_IN_WISHLIST");
                }

                await _unitOfWork.GetWriteRepository<WishlistItem>().HardDeleteAsync(wishlistItem);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Item removed from wishlist for user {UserId}.", request.UserId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing item from wishlist.");
                return Result<Unit>.Failure("An unexpected error occurred while removing item from wishlist.", StatusCodes.Status500InternalServerError, "WISHLIST_REMOVE_FAILED");
            }
        }
    }
}
