using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Dlc.Exceptions;
using PsStore.Application.Features.Dlc.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class DeleteDlcCommandHandler : BaseHandler, IRequestHandler<DeleteDlcCommandRequest, Unit>
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

        public async Task<Unit> Handle(DeleteDlcCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete DLC with ID {DlcId}", request.Id);

            await _dlcRules.DlcMustExist(request.Id);

            var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAsync(d => d.Id == request.Id, enableTracking: true);

            if (dlc.IsDeleted)
            {
                _logger.LogWarning("DLC with ID {DlcId} is already deleted.", request.Id);
                throw new DlcAlreadyDeletedException(request.Id);
            }

            await _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().SoftDeleteAsync(dlc);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully deleted DLC with ID {DlcId}", request.Id);

            return Unit.Value;
        }
    }
}
