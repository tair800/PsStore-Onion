using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Features.Wishlist.Commands.Add;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Wishlist.Commands.AddToWishlist
{
    public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddToWishlistCommandHandler> _logger;

        public AddToWishlistCommandHandler(IUnitOfWork unitOfWork, ILogger<AddToWishlistCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(AddToWishlistCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Get or create the wishlist
                var wishlist = await _unitOfWork.GetReadRepository<Domain.Entities.Wishlist>()
                    .GetAsync(w => w.UserId == request.UserId);

                if (wishlist == null)
                {
                    _logger.LogInformation("No wishlist found for user {UserId}, creating a new wishlist.", request.UserId);

                    wishlist = new Domain.Entities.Wishlist
                    {
                        UserId = request.UserId,
                    };

                    await _unitOfWork.GetWriteRepository<Domain.Entities.Wishlist>().AddAsync(wishlist);
                    await _unitOfWork.SaveAsync();
                }

                // Adding Game to Wishlist
                if (request.GameId.HasValue)
                {
                    var game = await _unitOfWork.GetReadRepository<Domain.Entities.Game>()
                        .GetAsync(g => g.Id == request.GameId.Value);

                    if (game == null)
                    {
                        _logger.LogWarning("Game with ID {GameId} not found.", request.GameId.Value);
                        return Result<Unit>.Failure("Game not found.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
                    }

                    var existingWishlistItem = await _unitOfWork.GetReadRepository<WishlistItem>()
                        .GetAsync(wi => wi.WishlistId == wishlist.Id && wi.GameId == game.Id);

                    if (existingWishlistItem != null)
                    {
                        _logger.LogInformation("Game with ID {GameId} is already in the wishlist. Skipping add.", game.Id);
                        return Result<Unit>.Failure("Game is already in the wishlist.", StatusCodes.Status400BadRequest, "GAME_ALREADY_IN_WISHLIST");
                    }

                    var wishlistItem = new WishlistItem
                    {
                        WishlistId = wishlist.Id,
                        GameId = game.Id,
                    };

                    await _unitOfWork.GetWriteRepository<WishlistItem>().AddAsync(wishlistItem);
                    wishlist.WishlistItems.Add(wishlistItem);
                }

                if (request.DlcId.HasValue)
                {
                    var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>()
                        .GetAsync(d => d.Id == request.DlcId.Value);

                    if (dlc == null)
                    {
                        _logger.LogWarning("DLC with ID {DlcId} not found.", request.DlcId.Value);
                        return Result<Unit>.Failure("DLC not found.", StatusCodes.Status404NotFound, "DLC_NOT_FOUND");
                    }

                    var existingWishlistItem = await _unitOfWork.GetReadRepository<WishlistItem>()
                        .GetAsync(wi => wi.WishlistId == wishlist.Id && wi.DlcId == dlc.Id);

                    if (existingWishlistItem != null)
                    {
                        _logger.LogInformation("DLC with ID {DlcId} is already in the wishlist. Skipping add.", dlc.Id);
                        return Result<Unit>.Failure("DLC is already in the wishlist.", StatusCodes.Status400BadRequest, "DLC_ALREADY_IN_WISHLIST");
                    }

                    var wishlistItem = new WishlistItem
                    {
                        WishlistId = wishlist.Id,
                        DlcId = dlc.Id,
                    };

                    await _unitOfWork.GetWriteRepository<WishlistItem>().AddAsync(wishlistItem);
                    wishlist.WishlistItems.Add(wishlistItem);
                }

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Game or DLC added to wishlist for user {UserId}.", request.UserId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding game or DLC to the wishlist.");
                return Result<Unit>.Failure("An unexpected error occurred while adding the game or DLC to the wishlist.", StatusCodes.Status500InternalServerError, "WISHLIST_ADDITION_FAILED");
            }
        }
    }
}
