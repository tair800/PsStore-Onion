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

                if (request.GameId.HasValue)
                {
                    var game = await _unitOfWork.GetReadRepository<Domain.Entities.Game>()
                        .GetAsync(g => g.Id == request.GameId.Value);

                    if (game == null)
                    {
                        _logger.LogWarning("Game with ID {GameId} not found.", request.GameId.Value);
                        return Result<Unit>.Failure("Game not found.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
                    }

                    var existingBasketGame = await _unitOfWork.GetReadRepository<BasketGame>()
                        .GetAsync(bg => bg.BasketId == basket.Id && bg.GameId == game.Id);

                    if (existingBasketGame != null)
                    {
                        _logger.LogInformation("Game with ID {GameId} is already in the basket. Skipping add.", game.Id);
                        return Result<Unit>.Failure("Game is already in the basket.", StatusCodes.Status400BadRequest, "GAME_ALREADY_IN_BASKET");
                    }


                    var basketGame = new BasketGame
                    {
                        BasketId = basket.Id,
                        GameId = game.Id,
                        Price = game.Price
                    };

                    await _unitOfWork.GetWriteRepository<BasketGame>().AddAsync(basketGame);
                    basket.BasketGames.Add(basketGame);
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

                    var existingBasketDlc = await _unitOfWork.GetReadRepository<BasketDlc>()
                        .GetAsync(bd => bd.BasketId == basket.Id && bd.DlcId == dlc.Id);

                    if (existingBasketDlc != null)
                    {
                        _logger.LogInformation("DLC with ID {DlcId} is already in the basket. Skipping add.", dlc.Id);
                        return Result<Unit>.Failure("DLC is already in the basket.", StatusCodes.Status400BadRequest, "DLC_ALREADY_IN_BASKET");
                    }

                    if (!request.GameId.HasValue && !request.DlcId.HasValue)
                    {
                        return Result<Unit>.Failure("Either GameId or DlcId must be provided.", StatusCodes.Status400BadRequest, "GAME_OR_DLC_REQUIRED");
                    }

                    if (request.GameId.HasValue && request.DlcId.HasValue)
                    {
                        return Result<Unit>.Failure("Cannot provide both GameId and DlcId. Provide only one.", StatusCodes.Status400BadRequest, "GAME_OR_DLC_CONFLICT");
                    }

                    var basketDlc = new BasketDlc
                    {
                        BasketId = basket.Id,
                        DlcId = dlc.Id,
                        Price = dlc.Price
                    };

                    await _unitOfWork.GetWriteRepository<BasketDlc>().AddAsync(basketDlc);
                    basket.BasketDlcs.Add(basketDlc);
                }

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Game or DLC added to basket for user {UserId}.", request.UserId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding game or DLC to the basket.");
                return Result<Unit>.Failure("An unexpected error occurred while adding the game or DLC to the basket.", StatusCodes.Status500InternalServerError, "BASKET_ADDITION_FAILED");
            }
        }
    }
}
