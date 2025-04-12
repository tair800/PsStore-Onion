using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Wishlist.Commands.Clear
{
    public class ClearWishlistCommandHandler : IRequestHandler<ClearWishlistCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClearWishlistCommandHandler> _logger;

        public ClearWishlistCommandHandler(IUnitOfWork unitOfWork, ILogger<ClearWishlistCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(ClearWishlistCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var wishlist = await _unitOfWork.GetReadRepository<Domain.Entities.Wishlist>()
                    .GetAsync(
                        w => w.UserId == request.UserId,
                        include: query => query.Include(w => w.WishlistItems)
                    );


                if (wishlist == null)
                {
                    _logger.LogWarning("No wishlist found for user {UserId}.", request.UserId);
                    return Result<Unit>.Failure("Wishlist not found.", StatusCodes.Status404NotFound, "WISHLIST_NOT_FOUND");
                }

                var items = await _unitOfWork.GetReadRepository<WishlistItem>()
                    .GetAllAsync(wi => wi.WishlistId == wishlist.Id);

                if (!items.Any())
                {
                    return Result<Unit>.Success(Unit.Value);
                }

                _unitOfWork.GetWriteRepository<WishlistItem>().HardDeleteRangeAsync(items);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Cleared wishlist for user {UserId}.", request.UserId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while clearing the wishlist.");
                return Result<Unit>.Failure("An unexpected error occurred while clearing the wishlist.", StatusCodes.Status500InternalServerError, "CLEAR_WISHLIST_FAILED");
            }
        }
    }
}
