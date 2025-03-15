using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Game.Exceptions;
using PsStore.Application.Features.Game.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Game.Commands
{
    public class UpdateGameCommandHandler : BaseHandler, IRequestHandler<UpdateGameCommandRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly GameRules _gameRules;
        private readonly ILogger<UpdateGameCommandHandler> _logger;

        public UpdateGameCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            GameRules gameRules,
            ILogger<UpdateGameCommandHandler> logger)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _gameRules = gameRules;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateGameCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update Game with ID {GameId}", request.Id);

            var game = await _unitOfWork.GetReadRepository<Domain.Entities.Game>()
                .GetAsync(g => g.Id == request.Id, enableTracking: true, include: q => q.Include(g => g.Category));

            if (game == null)
            {
                _logger.LogWarning("Game with ID {GameId} not found.", request.Id);
                throw new GameNotFoundException(request.Id);
            }


            // 🔹 Ensure the new title is unique
            if (!string.IsNullOrEmpty(request.Title) && request.Title != game.Title)
            {
                await _gameRules.GameTitleMustBeUnique(request.Title);
            }

            // 🔹 Ensure platform exists
            if (request.PlatformId.HasValue)
            {
                await _gameRules.PlatformMustExist(request.PlatformId.Value);
            }

            if (request.CategoryId.HasValue && request.CategoryId.Value != game.CategoryId)
            {
                _logger.LogInformation("Updating Game ID {GameId} with new CategoryId {CategoryId}", request.Id, request.CategoryId);
                await _gameRules.CategoryMustExist(request.CategoryId.Value);

                game.CategoryId = request.CategoryId.Value;
                _unitOfWork.GetWriteRepository<Domain.Entities.Game>().MarkAsModified(game, g => g.CategoryId);
            }
            else
            {
                _logger.LogInformation("CategoryId is not provided or unchanged, skipping update.");
            }






            List<Domain.Entities.Dlc>? dlcs = null;
            if (request.DlcIds is not null && request.DlcIds.Any())
            {
                dlcs = (await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>()
                    .GetAllAsync(d => request.DlcIds.Contains(d.Id)))
                    .ToList();

                if (dlcs.Count != request.DlcIds.Count)
                {
                    _logger.LogWarning("Some DLCs were not found. Requested: {RequestedDlcIds}, Found: {FoundDlcIds}",
                        string.Join(",", request.DlcIds), string.Join(",", dlcs.Select(d => d.Id)));

                    throw new DlcNotFoundException();
                }
            }

            // 🔹 Map the request data to the entity
            _mapper.Map(request, game);

            var writeRepo = _unitOfWork.GetWriteRepository<Domain.Entities.Game>();

            // Only mark fields as modified if they are present in the request
            if (request.Title is not null) writeRepo.MarkAsModified(game, g => g.Title);
            if (request.Description is not null) writeRepo.MarkAsModified(game, g => g.Description);
            if (request.Price is not null) writeRepo.MarkAsModified(game, g => g.Price);
            if (request.SalePrice is not null) writeRepo.MarkAsModified(game, g => g.SalePrice);
            if (request.ImgUrl is not null) writeRepo.MarkAsModified(game, g => g.ImgUrl);
            if (request.CategoryId is not null) writeRepo.MarkAsModified(game, g => g.CategoryId);
            if (request.PlatformId is not null) writeRepo.MarkAsModified(game, g => g.Platform);
            if (dlcs is not null) writeRepo.MarkAsModified(game, g => g.Dlcs);

            game.UpdatedDate = DateTime.UtcNow;
            writeRepo.MarkAsModified(game, g => g.UpdatedDate);


            try
            {
                var sql = _unitOfWork.GetWriteRepository<Domain.Entities.Game>().GenerateSqlForUpdate(game);
                _logger.LogInformation("Executing SQL: {Sql}", sql);  // Logs the SQL query

                await _unitOfWork.GetWriteRepository<Domain.Entities.Game>().UpdateAsync(game);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully updated Game with ID {GameId}", request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Game with ID {GameId}. CategoryId: {CategoryId}", request.Id, game.CategoryId);
                throw new GameUpdateFailedException(request.Id);
            }


            return Unit.Value;
        }
    }
}
