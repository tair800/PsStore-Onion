using MediatR;
using Microsoft.AspNetCore.Http;
using PsStore.Application.Bases;
using PsStore.Application.Features.Dlc.Exceptions;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class RestoreDlcCommandHandler : BaseHandler, IRequestHandler<RestoreDlcCommandRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RestoreDlcCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RestoreDlcCommandRequest request, CancellationToken cancellationToken)
        {
            var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAsync(d => d.Id == request.Id, includeDeleted: true);
            if (dlc == null)
            {
                throw new DlcNotFoundException(request.Id);
            }

            await _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().RestoreAsync(dlc);

            return Unit.Value;
        }
    }
}
