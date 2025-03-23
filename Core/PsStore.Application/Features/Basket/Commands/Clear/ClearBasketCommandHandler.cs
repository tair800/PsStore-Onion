using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Basket.Commands.Clear
{
    public class ClearBasketCommandHandler : IRequestHandler<ClearBasketCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClearBasketCommandHandler> _logger;

        public ClearBasketCommandHandler(IUnitOfWork unitOfWork, ILogger<ClearBasketCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(ClearBasketCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var basketGames = await _unitOfWork.GetReadRepository<BasketGame>()
                    .Find(bg => bg.Basket.UserId == request.UserId).ToListAsync();

                if (basketGames.Count == 0)
                {
                    return Result<Unit>.Failure("No items found in the basket.", StatusCodes.Status404NotFound, "NO_ITEMS_FOUND");
                }

                await _unitOfWork.GetWriteRepository<BasketGame>().HardDeleteRangeAsync(basketGames);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully cleared the basket for User {UserId}", request.UserId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear the basket for User {UserId}", request.UserId);
                return Result<Unit>.Failure("An error occurred while clearing the basket.", StatusCodes.Status500InternalServerError, "CLEAR_BASKET_FAILED");
            }
        }
    }

}
