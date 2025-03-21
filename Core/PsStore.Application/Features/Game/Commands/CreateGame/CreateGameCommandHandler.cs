using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Game.Commands.CreateGame;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class CreateGameCommandHandler : BaseHandler, IRequestHandler<CreateGameCommandRequest, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GameRules _gameRules;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateGameCommandHandler> _logger;

    public CreateGameCommandHandler(
        GameRules gameRules,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CreateGameCommandHandler> logger)
        : base(mapper, unitOfWork, httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _gameRules = gameRules;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(CreateGameCommandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling CreateGameCommand for title: {Title}", request.Title);

            var titleCheckResult = await _gameRules.GameTitleMustBeUnique(request.Title);
            if (!titleCheckResult.IsSuccess)
                return titleCheckResult;

            var platformCheckResult = await _gameRules.PlatformMustExist(request.PlatformId);
            if (!platformCheckResult.IsSuccess)
                return platformCheckResult;

            var categoryCheckResult = await _gameRules.CategoryMustExist(request.CategoryId);
            if (!categoryCheckResult.IsSuccess)
                return categoryCheckResult;

            List<Dlc> dlcs = new();
            if (request.DlcIds?.Any() == true)
            {
                dlcs = (await _unitOfWork.GetReadRepository<Dlc>()
                    .GetAllAsync(d => request.DlcIds.Contains(d.Id)))
                    .ToList();

                // if (dlcs.Count != request.DlcIds.Count)
                // {
                //     _logger.LogWarning("Some DLCs were not found. Requested: {RequestedDlcIds}, Found: {FoundDlcIds}",
                //         string.Join(",", request.DlcIds), string.Join(",", dlcs.Select(d => d.Id)));
                //     return Result<Unit>.Failure("Some DLCs were not found.", StatusCodes.Status404NotFound, "DLC_NOT_FOUND");
                // }
            }

            _logger.LogInformation("Creating new game: Title={Title}, CategoryId={CategoryId}, Price={Price}, Platform={Platform}",
                request.Title, request.CategoryId, request.Price, (Platform)request.PlatformId);

            Game game = _mapper.Map<Game>(request);
            game.Platform = (Platform)request.PlatformId;
            if (dlcs.Any())
            {
                game.Dlcs = dlcs;
            }

            await _unitOfWork.GetWriteRepository<Game>().AddAsync(game);

            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully created game with ID {GameId}", game.Id);
            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating the game.");
            return Result<Unit>.Failure("An unexpected error occurred while creating the game.", StatusCodes.Status500InternalServerError, "GAME_CREATION_FAILED");
        }
    }
}
