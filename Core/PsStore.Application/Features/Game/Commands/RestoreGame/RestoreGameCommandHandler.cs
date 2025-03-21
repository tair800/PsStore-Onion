using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Game.Commands
{
    public class RestoreGameCommandHandler : BaseHandler, IRequestHandler<RestoreGameCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RestoreGameCommandHandler> _logger;

        public RestoreGameCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RestoreGameCommandHandler> logger)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(RestoreGameCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to restore Game with ID {GameId}", request.Id);

            var game = await _unitOfWork.GetReadRepository<Domain.Entities.Game>().GetAsync(g => g.Id == request.Id, includeDeleted: true, enableTracking: true);

            if (game == null)
            {
                _logger.LogWarning("Game with ID {GameId} not found.", request.Id);
                return Result<Unit>.Failure("Game not found.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
            }

            if (!game.IsDeleted)
            {
                _logger.LogWarning("Game with ID {GameId} is already active.", request.Id);
                return Result<Unit>.Failure("Game is already active.", StatusCodes.Status400BadRequest, "GAME_ALREADY_ACTIVE");
            }

            await _unitOfWork.GetWriteRepository<Domain.Entities.Game>().RestoreAsync(game);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully restored Game with ID {GameId}", request.Id);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
