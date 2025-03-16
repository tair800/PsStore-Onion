using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Game.Exceptions;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Game.Commands
{
    public class RestoreGameCommandHandler : BaseHandler, IRequestHandler<RestoreGameCommandRequest, Unit>
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

        public async Task<Unit> Handle(RestoreGameCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to restore Game with ID {GameId}", request.Id);

            var game = await _unitOfWork.GetReadRepository<Domain.Entities.Game>().GetAsync(g => g.Id == request.Id, includeDeleted: true, enableTracking: true);

            if (game == null)
            {
                _logger.LogWarning("Game with ID {GameId} not found.", request.Id);
                throw new GameNotFoundException(request.Id);
            }

            if (!game.IsDeleted)
            {
                _logger.LogWarning("Game with ID {GameId} is already active.", request.Id);
                throw new GameAlreadyActiveException(request.Id);
            }

            await _unitOfWork.GetWriteRepository<Domain.Entities.Game>().RestoreAsync(game);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully restored Game with ID {GameId}", request.Id);

            return Unit.Value;
        }
    }
}