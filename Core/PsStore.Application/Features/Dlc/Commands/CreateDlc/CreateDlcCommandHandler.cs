using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Dlc.Commands.CreateDlc;
using PsStore.Application.Features.Dlc.Exceptions;
using PsStore.Application.Features.Dlc.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class CreateDlcCommandHandler : BaseHandler, IRequestHandler<CreateDlcCommandRequest, Unit>
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

        public async Task<Unit> Handle(CreateDlcCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new DLC: {DlcName} for Game ID {GameId}", request.Name, request.GameId);

            _dlcRules.PriceMustBeValid(request.Price);
            await _dlcRules.GameMustExist(request.GameId);
            await _dlcRules.DlcNameMustBeUnique(request.GameId, request.Name);

            var dlc = _mapper.Map<Domain.Entities.Dlc>(request);

            await _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().AddAsync(dlc);

            try
            {
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Successfully created DLC with ID {DlcId}", dlc.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving DLC.");
                throw new DlcCreationFailedException(request.Name);
            }

            return Unit.Value;
        }
    }
}
