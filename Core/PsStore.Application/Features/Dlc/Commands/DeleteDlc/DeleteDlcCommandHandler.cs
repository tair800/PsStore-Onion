using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class DeleteDlcCommandHandler : BaseHandler, IRequestHandler<DeleteDlcCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DlcRules _dlcRules;
        private readonly ILogger<DeleteDlcCommandHandler> _logger;

        public DeleteDlcCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, DlcRules dlcRules, ILogger<DeleteDlcCommandHandler> logger)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _dlcRules = dlcRules;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(DeleteDlcCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete DLC with ID {DlcId}", request.Id);

            // Validate if DLC exists
            var dlcValidation = await _dlcRules.DlcMustExist(request.Id);
            if (!dlcValidation.IsSuccess)
            {
                return Result<Unit>.Failure(dlcValidation.Error, StatusCodes.Status404NotFound, "DLC_NOT_FOUND");
            }

            var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAsync(d => d.Id == request.Id, enableTracking: true);

            // If the DLC is already deleted
            if (dlc.IsDeleted)
            {
                _logger.LogWarning("DLC with ID {DlcId} is already deleted.", request.Id);
                return Result<Unit>.Failure("DLC is already deleted.", StatusCodes.Status409Conflict, "DLC_ALREADY_DELETED");
            }

            // Perform soft delete on the DLC
            await _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().SoftDeleteAsync(dlc);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully deleted DLC with ID {DlcId}", request.Id);

            return Result<Unit>.Success(Unit.Value); // Return success
        }
    }
}
