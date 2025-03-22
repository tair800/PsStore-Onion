using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Basket.Queries.GetBasket
{
    public class GetBasketCommandHandler : IRequestHandler<GetBasketCommandRequest, Result<GetBasketCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetBasketCommandHandler> _logger;

        public GetBasketCommandHandler(IUnitOfWork unitOfWork, ILogger<GetBasketCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<GetBasketCommandResponse>> Handle(GetBasketCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Get the user's basket
                var basket = await _unitOfWork.GetReadRepository<Domain.Entities.Basket>()
                    .GetAsync(b => b.UserId == request.UserId);

                if (basket == null)
                {
                    _logger.LogWarning("No basket found for user {UserId}.", request.UserId);
                    return Result<GetBasketCommandResponse>.Failure("Basket not found.", StatusCodes.Status404NotFound, "BASKET_NOT_FOUND");
                }

                // Get all the basket games for the user, including the Game entity
                var basketGames = await _unitOfWork.GetReadRepository<BasketGame>()
                    .GetAllAsync(bg => bg.BasketId == basket.Id, include: query => query.Include(bg => bg.Game));

                // Map BasketGame entities to response model
                var basketGameResponses = basketGames.Select(bg => new GetBasketGameResponse
                {
                    GameId = bg.GameId,
                    GameTitle = bg.Game.Title,
                    Price = bg.Price
                }).ToList();

                // Calculate the total price of the basket
                var totalPrice = basketGames.Sum(bg => bg.Price);

                // Prepare the response
                var response = new GetBasketCommandResponse
                {
                    UserId = basket.UserId,
                    BasketGames = basketGameResponses,
                    TotalPrice = totalPrice
                };

                _logger.LogInformation("Successfully retrieved basket for user {UserId} with {GameCount} games.", request.UserId, basketGames.Count);
                return Result<GetBasketCommandResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the basket for user {UserId}.", request.UserId);
                return Result<GetBasketCommandResponse>.Failure("An unexpected error occurred while retrieving the basket.", StatusCodes.Status500InternalServerError, "BASKET_RETRIEVAL_FAILED");
            }
        }
    }
}
