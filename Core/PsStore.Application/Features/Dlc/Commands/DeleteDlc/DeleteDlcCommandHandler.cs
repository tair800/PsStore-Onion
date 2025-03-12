using MediatR;
using Microsoft.AspNetCore.Http;
using PsStore.Application.Bases;
using PsStore.Application.Features.Dlc.Exceptions;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class DeleteDlcCommandHandler : BaseHandler, IRequestHandler<DeleteDlcCommandRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDlcCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteDlcCommandRequest request, CancellationToken cancellationToken)
        {
            var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAsync(d => d.Id == request.Id);
            if (dlc == null)
            {
                throw new DlcNotFoundException(request.Id);
            }

            await _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().SoftDeleteAsync(dlc);

            return Unit.Value;
        }
    }
}
