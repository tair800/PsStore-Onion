using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PsStore.Application.Interfaces.UnitOfWorks;

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
                // Get all baskets, including the BasketGames and Game and DLC entities
                var baskets = await _unitOfWork.GetReadRepository<Domain.Entities.Basket>()
                    .GetAllAsync(include: query => query
                        .Include(b => b.BasketGames)
                        .ThenInclude(bg => bg.Game)
                        .ThenInclude(g => g.Dlcs), enableTracking: false);

                if (!baskets.Any())
                {
                    _logger.LogWarning("No baskets found.");
                    return Result<GetAllBasketsCommandResponse>.Failure("No baskets found.", StatusCodes.Status404NotFound, "BASKETS_NOT_FOUND");
                }

                // Map the basket data into response DTO
                var basketResponses = baskets.Select(basket => new GetAllBasketsCommandResponse
                {
                    UserId = basket.UserId,
                    BasketId = basket.Id,
                    BasketGames = basket.BasketGames.Select(bg => new GetBasketGameResponse
                    {
                        GameId = bg.GameId,
                        GameTitle = bg.Game.Title,
                        Price = bg.Price,
                        Dlcs = bg.Game.Dlcs.Select(dlc => new GetBasketDlcResponse
                        {
                            DlcId = dlc.Id,
                            DlcName = dlc.Name,
                            DlcPrice = dlc.Price
                        }).ToList()
                    }).ToList(),
                    TotalPrice = basket.BasketGames.Sum(bg => bg.Price) + basket.BasketGames
                        .SelectMany(bg => bg.Game.Dlcs)
                        .Sum(dlc => dlc.Price)
                }).ToList();

                _logger.LogInformation("Successfully retrieved all baskets.");
                return Result<GetAllBasketsCommandResponse>.Success(new GetAllBasketsCommandResponse
                {
                    Baskets = basketResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all baskets.");
                return Result<GetAllBasketsCommandResponse>.Failure("An unexpected error occurred while retrieving the baskets.", StatusCodes.Status500InternalServerError, "BASKET_RETRIEVAL_FAILED");
            }
        }
    }
}
