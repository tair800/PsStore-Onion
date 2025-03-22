using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Basket.Commands.AddToBasket
{
    public class AddToBasketCommandHandler : IRequestHandler<AddToBasketCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddToBasketCommandHandler> _logger;

        public AddToBasketCommandHandler(IUnitOfWork unitOfWork, ILogger<AddToBasketCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(AddToBasketCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the user's basket, or create a new one if not found
                var basket = await _unitOfWork.GetReadRepository<Domain.Entities.Basket>()
                    .GetAsync(b => b.UserId == request.UserId);

                if (basket == null)
                {
                    _logger.LogInformation("No basket found for user {UserId}, creating a new basket.", request.UserId);

                    basket = new Domain.Entities.Basket
                    {
                        UserId = request.UserId,
                    };

                    await _unitOfWork.GetWriteRepository<Domain.Entities.Basket>().AddAsync(basket);
                    await _unitOfWork.SaveAsync();
                }

                // Retrieve the game from the repository
                var game = await _unitOfWork.GetReadRepository<Domain.Entities.Game>()
                    .GetAsync(g => g.Id == request.GameId);

                if (game == null)
                {
                    _logger.LogWarning("Game with ID {GameId} not found.", request.GameId);
                    return Result<Unit>.Failure("Game not found.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
                }

                // Check if the game is already in the basket
                var existingBasketGame = await _unitOfWork.GetReadRepository<BasketGame>()
                    .GetAsync(bg => bg.BasketId == basket.Id && bg.GameId == game.Id);

                if (existingBasketGame != null)
                {
                    _logger.LogInformation("Game with ID {GameId} is already in the basket. Skipping add.", game.Id);
                    return Result<Unit>.Failure("Game is already in the basket.", StatusCodes.Status400BadRequest, "GAME_ALREADY_IN_BASKET");
                }

                // Create a new BasketGame entry
                var basketGame = new BasketGame
                {
                    BasketId = basket.Id,
                    GameId = game.Id,
                    Price = game.Price // Price from the game entity
                };

                await _unitOfWork.GetWriteRepository<BasketGame>().AddAsync(basketGame);
                basket.BasketGames.Add(basketGame); // Add the game to the basket's list of games

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Game with ID {GameId} added to basket for user {UserId}.", request.GameId, request.UserId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding game to the basket.");
                return Result<Unit>.Failure("An unexpected error occurred while adding the game to the basket.", StatusCodes.Status500InternalServerError, "GAME_ADDITION_FAILED");
            }
        }
    }
}
