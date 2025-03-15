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
    public class UpdateDlcCommandHandler : BaseHandler, IRequestHandler<UpdateDlcCommandRequest, Unit>
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

        public async Task<Unit> Handle(UpdateDlcCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update DLC with ID {DlcId}", request.Id);

            var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAsync(d => d.Id == request.Id, enableTracking: true);
            if (dlc == null)
            {
                _logger.LogWarning("DLC with ID {DlcId} not found.", request.Id);
                throw new DlcNotFoundException();
            }

            await _dlcRules.GameMustExist(request.GameId);

            if (request.Name != dlc.Name)
            {
                await _dlcRules.DlcNameMustBeUnique(request.GameId, request.Name);
            }

            _dlcRules.PriceMustBeValid(request.Price);

            _mapper.Map(request, dlc);
            dlc.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().UpdateAsync(dlc);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Successfully updated DLC with ID {DlcId}", request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating DLC with ID {DlcId}", request.Id);
                throw new DlcUpdateFailedException(request.Id);
            }

            return Unit.Value;
        }
    }
}
