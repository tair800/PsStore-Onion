using MediatR;
using Microsoft.AspNetCore.Http;
using PsStore.Application.Bases;
using PsStore.Application.Features.Dlc.Exceptions;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class UpdateDlcCommandHandler : BaseHandler, IRequestHandler<UpdateDlcCommandRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateDlcCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateDlcCommandRequest request, CancellationToken cancellationToken)
        {
            var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAsync(d => d.Id == request.Id);

            if (dlc == null)
            {
                throw new DlcNotFoundException(request.Id);
            }

            _mapper.Map(request, dlc);
            dlc.UpdatedDate = DateTime.Now;

            await _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().UpdateAsync(dlc);
            await _unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
