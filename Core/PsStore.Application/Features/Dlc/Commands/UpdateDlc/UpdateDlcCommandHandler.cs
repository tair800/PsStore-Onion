using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Dlc.Commands;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class UpdateDlcCommandHandler : BaseHandler, IRequestHandler<UpdateDlcCommandRequest, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly DlcRules _dlcRules;
    private readonly ILogger<UpdateDlcCommandHandler> _logger;

    public UpdateDlcCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        DlcRules dlcRules,
        ILogger<UpdateDlcCommandHandler> logger)
        : base(mapper, unitOfWork, httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dlcRules = dlcRules;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(UpdateDlcCommandRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to update DLC with ID {DlcId}", request.Id);

        var dlc = await _unitOfWork.GetReadRepository<Dlc>()
            .GetAsync(d => d.Id == request.Id, enableTracking: true);

        if (dlc == null)
        {
            _logger.LogWarning("DLC with ID {DlcId} not found.", request.Id);
            return Result<Unit>.Failure("DLC not found.", StatusCodes.Status404NotFound, "DLC_NOT_FOUND");
        }

        // Treat 0 as null for GameId
        int? gameIdToUpdate = request.GameId == 0 ? null : request.GameId;

        // Update GameId only if it's provided and not 0
        if (gameIdToUpdate.HasValue && gameIdToUpdate.Value != dlc.GameId)
        {
            var gameCheckResult = await _dlcRules.GameMustExist(gameIdToUpdate.Value);
            if (!gameCheckResult.IsSuccess) return gameCheckResult;

            dlc.GameId = gameIdToUpdate.Value;
            _unitOfWork.GetWriteRepository<Dlc>().MarkAsModified(dlc, d => d.GameId);
        }

        _mapper.Map(request, dlc);

        var writeRepo = _unitOfWork.GetWriteRepository<Dlc>();

        if (request.Name is not null) writeRepo.MarkAsModified(dlc, d => d.Name);
        if (request.Description is not null) writeRepo.MarkAsModified(dlc, d => d.Description);
        if (request.Price is not null) writeRepo.MarkAsModified(dlc, d => d.Price);
        if (request.SalePrice is not null) writeRepo.MarkAsModified(dlc, d => d.SalePrice);
        if (request.ImgUrl is not null) writeRepo.MarkAsModified(dlc, d => d.ImgUrl);
        if (gameIdToUpdate.HasValue) writeRepo.MarkAsModified(dlc, d => d.GameId);

        dlc.UpdatedDate = DateTime.UtcNow;
        writeRepo.MarkAsModified(dlc, d => d.UpdatedDate);

        try
        {
            var sql = _unitOfWork.GetWriteRepository<Dlc>().GenerateSqlForUpdate(dlc);
            _logger.LogInformation("Executing SQL: {Sql}", sql);

            await _unitOfWork.GetWriteRepository<Dlc>().UpdateAsync(dlc);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully updated DLC with ID {DlcId}", request.Id);
            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update DLC with ID {DlcId}.", request.Id);
            return Result<Unit>.Failure("Failed to update DLC.", StatusCodes.Status500InternalServerError, "DLC_UPDATE_FAILED");
        }
    }

}
