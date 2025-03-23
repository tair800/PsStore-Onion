using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Game.Commands;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class UpdateGameCommandHandler : BaseHandler, IRequestHandler<UpdateGameCommandRequest, Result<Unit>>
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

    public async Task<Result<Unit>> Handle(UpdateGameCommandRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to update Game with ID {GameId}", request.Id);

        var game = await _unitOfWork.GetReadRepository<Game>()
            .GetAsync(g => g.Id == request.Id, enableTracking: true, include: q => q.Include(g => g.Category));

        if (game == null)
        {
            _logger.LogWarning("Game with ID {GameId} not found.", request.Id);
            return Result<Unit>.Failure("Game not found.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
        }

        if (request.CategoryId.HasValue && request.CategoryId.Value != 0)
        {
            var categoryCheckResult = await _gameRules.CategoryMustExist(request.CategoryId.Value);
            if (!categoryCheckResult.IsSuccess) return categoryCheckResult;

            game.CategoryId = request.CategoryId.Value;
            _unitOfWork.GetWriteRepository<Game>().MarkAsModified(game, g => g.CategoryId);
        }
        else
        {
            _logger.LogInformation("CategoryId is not provided or unchanged, skipping update.");
        }

        if (!string.IsNullOrEmpty(request.Title))
        {
            game.Title = request.Title;
            _unitOfWork.GetWriteRepository<Game>().MarkAsModified(game, g => g.Title);
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            game.Description = request.Description;
            _unitOfWork.GetWriteRepository<Game>().MarkAsModified(game, g => g.Description);
        }

        if (request.Price.HasValue)
        {
            game.Price = request.Price.Value;
            _unitOfWork.GetWriteRepository<Game>().MarkAsModified(game, g => g.Price);
        }

        if (request.SalePrice.HasValue)
        {
            game.SalePrice = request.SalePrice.Value;
            _unitOfWork.GetWriteRepository<Game>().MarkAsModified(game, g => g.SalePrice);
        }

        if (!string.IsNullOrEmpty(request.ImgUrl))
        {
            game.ImgUrl = request.ImgUrl;
            _unitOfWork.GetWriteRepository<Game>().MarkAsModified(game, g => g.ImgUrl);
        }

        game.UpdatedDate = DateTime.UtcNow;
        _unitOfWork.GetWriteRepository<Game>().MarkAsModified(game, g => g.UpdatedDate);

        try
        {
            var sql = _unitOfWork.GetWriteRepository<Game>().GenerateSqlForUpdate(game);
            _logger.LogInformation("Executing SQL: {Sql}", sql);

            await _unitOfWork.GetWriteRepository<Game>().UpdateAsync(game);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully updated Game with ID {GameId}", request.Id);
            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update Game with ID {GameId}.", request.Id);
            return Result<Unit>.Failure("Failed to update game.", StatusCodes.Status500InternalServerError, "GAME_UPDATE_FAILED");
        }
    }


}
