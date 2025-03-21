using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Dlc.Commands.CreateDlc;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class CreateDlcCommandHandler : BaseHandler, IRequestHandler<CreateDlcCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DlcRules _dlcRules;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateDlcCommandHandler> _logger;

        public CreateDlcCommandHandler(
            DlcRules dlcRules,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateDlcCommandHandler> logger)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _dlcRules = dlcRules;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(CreateDlcCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new DLC: {DlcName} for Game ID {GameId}", request.Name, request.GameId);

            var priceValidation = _dlcRules.PriceMustBeValid(request.Price);
            if (!priceValidation.IsSuccess)
            {
                return Result<Unit>.Failure(priceValidation.Error, StatusCodes.Status400BadRequest, "INVALID_PRICE");
            }

            var gameValidation = await _dlcRules.GameMustExist(request.GameId);
            if (!gameValidation.IsSuccess)
            {
                return Result<Unit>.Failure(gameValidation.Error, StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
            }

            var nameValidation = await _dlcRules.DlcNameMustBeUnique(request.GameId, request.Name);
            if (!nameValidation.IsSuccess)
            {
                return Result<Unit>.Failure(nameValidation.Error, StatusCodes.Status409Conflict, "DLC_NAME_ALREADY_EXISTS");
            }

            var dlc = _mapper.Map<Domain.Entities.Dlc>(request);

            await _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().AddAsync(dlc);

            try
            {
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Successfully created DLC with ID {DlcId}", dlc.Id);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving DLC.");
                return Result<Unit>.Failure("An error occurred while creating the DLC.", StatusCodes.Status500InternalServerError, "DLC_CREATION_FAILED");
            }
        }
    }
}
