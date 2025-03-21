using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Game.Commands
{
    public class DeleteGameCommandHandler : BaseHandler, IRequestHandler<DeleteGameCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly GameRules _gameRules;
        private readonly ILogger<DeleteGameCommandHandler> _logger;

        public DeleteGameCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            GameRules gameRules,
            ILogger<DeleteGameCommandHandler> logger)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _gameRules = gameRules;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(DeleteGameCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete Game with ID {GameId}", request.Id);

            var gameCheckResult = await _gameRules.GameMustExist(request.Id);
            if (!gameCheckResult.IsSuccess)
                return gameCheckResult;

            var game = await _unitOfWork.GetReadRepository<Domain.Entities.Game>()
                .GetAsync(g => g.Id == request.Id, enableTracking: true);

            if (game.IsDeleted)
            {
                _logger.LogWarning("Game with ID {GameId} is already deleted.", request.Id);
                return Result<Unit>.Failure("Game is already deleted.", StatusCodes.Status400BadRequest, "GAME_ALREADY_DELETED");
            }

            await _unitOfWork.GetWriteRepository<Domain.Entities.Game>().SoftDeleteAsync(game);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully deleted Game with ID {GameId}", request.Id);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
