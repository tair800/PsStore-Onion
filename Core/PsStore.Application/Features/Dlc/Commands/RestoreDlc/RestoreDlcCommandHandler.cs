using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class RestoreDlcCommandHandler : BaseHandler, IRequestHandler<RestoreDlcCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RestoreDlcCommandHandler> _logger;

        public RestoreDlcCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RestoreDlcCommandHandler> logger)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(RestoreDlcCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to restore DLC with ID {DlcId}", request.Id);

            var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAsync(d => d.Id == request.Id, includeDeleted: true, enableTracking: true);

            if (dlc == null)
            {
                _logger.LogWarning("DLC with ID {DlcId} not found.", request.Id);
                return Result<Unit>.Failure("DLC not found.", StatusCodes.Status404NotFound, "DLC_NOT_FOUND");
            }

            if (!dlc.IsDeleted)
            {
                _logger.LogWarning("DLC with ID {DlcId} is already active.", request.Id);
                return Result<Unit>.Failure("DLC is already active.", StatusCodes.Status409Conflict, "DLC_ALREADY_ACTIVE");
            }

            await _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().RestoreAsync(dlc);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully restored DLC with ID {DlcId}", request.Id);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
