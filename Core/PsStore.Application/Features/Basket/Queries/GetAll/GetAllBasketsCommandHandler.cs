using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PsStore.Application.Features.Basket.Queries.GetAll;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Basket.Queries.GetAllBaskets
{
    public class GetAllBasketsCommandHandler : IRequestHandler<GetAllBasketsCommandRequest, Result<GetAllBasketsCommandResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllBasketsCommandHandler> _logger;

        public GetAllBasketsCommandHandler(IUnitOfWork unitOfWork, ILogger<GetAllBasketsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<GetAllBasketsCommandResponse>> Handle(GetAllBasketsCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var baskets = await _unitOfWork.GetReadRepository<Domain.Entities.Basket>()
                    .GetAllAsync(enableTracking: false);

                if (!baskets.Any())
                {
                    _logger.LogWarning("No baskets found.");
                    return Result<GetAllBasketsCommandResponse>.Failure("No baskets found.", StatusCodes.Status404NotFound, "BASKETS_NOT_FOUND");
                }

                var basketGames = await _unitOfWork.GetReadRepository<BasketGame>()
                    .GetAllAsync(bg => baskets.Select(b => b.Id).Contains(bg.BasketId), include: query => query.Include(bg => bg.Game));

                var basketGameResponses = basketGames.Select(bg => new GetAll.GetBasketGameResponse
                {
                    BasketId = bg.BasketId,
                    GameId = bg.GameId,
                    GameTitle = bg.Game.Title,
                    Price = bg.Price
                }).ToList();


                var basketsResponse = baskets.Select(basket => new GetBasketResponse
                {
                    UserId = basket.UserId,
                    BasketId = basket.Id,
                    BasketGames = basketGameResponses.Where(bg => bg.BasketId == basket.Id).ToList(),
                    TotalPrice = basketGameResponses.Where(bg => bg.BasketId == basket.Id).Sum(bg => bg.Price)
                }).ToList();

                var response = new GetAllBasketsCommandResponse
                {
                    Baskets = basketsResponse
                };

                _logger.LogInformation("Successfully retrieved all baskets.");
                return Result<GetAllBasketsCommandResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all baskets.");
                return Result<GetAllBasketsCommandResponse>.Failure("An unexpected error occurred while retrieving the baskets.", StatusCodes.Status500InternalServerError, "BASKET_RETRIEVAL_FAILED");
            }
        }
    }
}
