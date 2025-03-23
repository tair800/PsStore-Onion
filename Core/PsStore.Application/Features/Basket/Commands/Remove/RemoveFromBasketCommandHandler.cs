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
                var basketGame = await _unitOfWork.GetReadRepository<BasketGame>()
                    .GetAsync(bg => bg.Basket.UserId == request.UserId && bg.GameId == request.GameId);

                if (basketGame == null)
                {
                    return Result<Unit>.Failure("Game not found in the basket.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
                }

                await _unitOfWork.GetWriteRepository<BasketGame>().HardDeleteAsync(basketGame);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully removed game with ID {GameId} from basket for User {UserId}", request.GameId, request.UserId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove game from basket for User {UserId}", request.UserId);
                return Result<Unit>.Failure("An error occurred while removing the game from the basket.", StatusCodes.Status500InternalServerError, "REMOVE_GAME_FAILED");
            }
        }
    }

}
