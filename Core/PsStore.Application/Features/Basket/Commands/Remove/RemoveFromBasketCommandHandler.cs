using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Basket.Commands.Remove
{
    public class RemoveFromBasketCommandHandler : IRequestHandler<RemoveFromBasketCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoveFromBasketCommandHandler> _logger;

        public RemoveFromBasketCommandHandler(IUnitOfWork unitOfWork, ILogger<RemoveFromBasketCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(RemoveFromBasketCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var basket = await _unitOfWork.GetReadRepository<Domain.Entities.Basket>()
                    .GetAsync(b => b.UserId == request.UserId);

                if (basket == null)
                {
                    _logger.LogWarning("Basket not found for user {UserId}.", request.UserId);
                    return Result<Unit>.Failure("Basket not found.", StatusCodes.Status404NotFound, "BASKET_NOT_FOUND");
                }

                if (request.GameId.HasValue)
                {
                    var basketGame = await _unitOfWork.GetReadRepository<BasketGame>()
                        .GetAsync(bg => bg.BasketId == basket.Id && bg.GameId == request.GameId.Value);

                    if (basketGame == null)
                    {
                        _logger.LogWarning("Game with ID {GameId} not found in basket.", request.GameId.Value);
                        return Result<Unit>.Failure("Game not found in basket.", StatusCodes.Status404NotFound, "GAME_NOT_IN_BASKET");
                    }

                    await _unitOfWork.GetWriteRepository<BasketGame>().HardDeleteAsync(basketGame);
                }

                if (request.DlcId.HasValue)
                {
                    var basketDlc = await _unitOfWork.GetReadRepository<BasketDlc>()
                        .GetAsync(bd => bd.BasketId == basket.Id && bd.DlcId == request.DlcId.Value);

                    if (basketDlc == null)
                    {
                        _logger.LogWarning("DLC with ID {DlcId} not found in basket.", request.DlcId.Value);
                        return Result<Unit>.Failure("DLC not found in basket.", StatusCodes.Status404NotFound, "DLC_NOT_IN_BASKET");
                    }

                    await _unitOfWork.GetWriteRepository<BasketDlc>().HardDeleteAsync(basketDlc);
                }

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully removed items from basket for user {UserId}.", request.UserId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing items from the basket for user {UserId}.", request.UserId);
                return Result<Unit>.Failure("An error occurred while removing items from the basket.", StatusCodes.Status500InternalServerError, "REMOVE_ITEM_FAILED");
            }
        }
    }
}
