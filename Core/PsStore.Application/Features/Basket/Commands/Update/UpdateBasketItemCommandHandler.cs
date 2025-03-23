using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Basket.Commands.Update
{
    public class UpdateBasketItemCommandHandler : IRequestHandler<UpdateBasketItemCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBasketItemCommandHandler> _logger;

        public UpdateBasketItemCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateBasketItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(UpdateBasketItemCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var basketGame = await _unitOfWork.GetReadRepository<BasketGame>()
                    .GetAsync(bg => bg.Basket.UserId == request.UserId && bg.GameId == request.GameId);

                if (basketGame == null)
                {
                    return Result<Unit>.Failure("Game not found in the basket.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
                }

                basketGame.Price = request.Price;

                await _unitOfWork.GetWriteRepository<BasketGame>().UpdateAsync(basketGame);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully updated price for game with ID {GameId} in basket for User {UserId}", request.GameId, request.UserId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update game in basket for User {UserId}", request.UserId);
                return Result<Unit>.Failure("An error occurred while updating the game in the basket.", StatusCodes.Status500InternalServerError, "UPDATE_GAME_FAILED");
            }
        }
    }

}
