using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Game.Commands;
using PsStore.Application.Features.Game.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class CreateGameCommandHandler : BaseHandler, IRequestHandler<CreateGameCommandRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly GameRules _gameRules;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateGameCommandHandler> _logger;

    public CreateGameCommandHandler(GameRules gameRules, IMapper mapper, IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor, ILogger<CreateGameCommandHandler> logger)
        : base(mapper, unitOfWork, httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _gameRules = gameRules;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Unit> Handle(CreateGameCommandRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateGameCommand for title: {Title}", request.Title);

        await _gameRules.GameTitleMustBeUnique(request.Title);
        await _gameRules.PlatformMustExist(request.PlatformId);
        await _gameRules.CategoryMustExist(request.CategoryId);

        // Retrieve associated DLCs if provided
        List<Dlc> dlcs = new();
        if (request.DlcIds?.Any() == true)
        {
            dlcs = (await _unitOfWork.GetReadRepository<Dlc>()
                .GetAllAsync(d => request.DlcIds.Contains(d.Id)))
                .ToList();

            if (dlcs.Count != request.DlcIds.Count)
            {
                _logger.LogWarning("Some DLCs were not found. Requested: {RequestedDlcIds}, Found: {FoundDlcIds}",
                    string.Join(",", request.DlcIds), string.Join(",", dlcs.Select(d => d.Id)));

                throw new DlcNotFoundException();
            }
        }

        _logger.LogInformation("Creating new game: Title={Title}, CategoryId={CategoryId}, Price={Price}, Platform={Platform}",
            request.Title, request.CategoryId, request.Price, (Platform)request.PlatformId);

        Game game = _mapper.Map<Game>(request);
        game.Platform = (Platform)request.PlatformId;
        game.Dlcs = dlcs;

        await _unitOfWork.GetWriteRepository<Game>().AddAsync(game);

        try
        {
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Successfully created game with ID {GameId}", game.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving the game entity.");
            throw new GameCreationFailedException(request.Title);
        }

        return Unit.Value;
    }
}
