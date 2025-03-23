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
                var basket = await _unitOfWork.GetReadRepository<Domain.Entities.Basket>()
                    .GetAsync(b => b.UserId == request.UserId);

                if (basket == null)
                {
                    return Result<Unit>.Failure("Basket not found.", StatusCodes.Status404NotFound, "BASKET_NOT_FOUND");
                }

                var basketGame = await _unitOfWork.GetReadRepository<BasketGame>()
                    .GetAsync(bg => bg.BasketId == basket.Id && bg.GameId == request.GameId);

                if (basketGame != null)
                {
                    basketGame.Price = request.GamePrice;

                    await _unitOfWork.GetWriteRepository<BasketGame>().UpdateAsync(basketGame);
                }
                else
                {
                    return Result<Unit>.Failure("Game not found in the basket.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
                }

                if (request.DlcPrices != null && request.DlcPrices.Count > 0)
                {
                    foreach (var dlcPrice in request.DlcPrices)
                    {
                        var basketDlc = await _unitOfWork.GetReadRepository<BasketDlc>()
                            .GetAsync(bd => bd.BasketId == basket.Id && bd.DlcId == dlcPrice.DlcId);

                        if (basketDlc != null)
                        {
                            basketDlc.Price = dlcPrice.Price;

                            await _unitOfWork.GetWriteRepository<BasketDlc>().UpdateAsync(basketDlc);
                        }
                        else
                        {
                            var newBasketDlc = new BasketDlc
                            {
                                BasketId = basket.Id,
                                DlcId = dlcPrice.DlcId,
                                Price = dlcPrice.Price
                            };

                            await _unitOfWork.GetWriteRepository<BasketDlc>().AddAsync(newBasketDlc);
                        }
                    }
                }

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully updated basket for user {UserId}", request.UserId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the basket for user {UserId}.", request.UserId);
                return Result<Unit>.Failure("An error occurred while updating the basket.", StatusCodes.Status500InternalServerError, "BASKET_UPDATE_FAILED");
            }
        }
    }
}
