using MediatR;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Queries.GetAllDlc
{
    public class GetAllDlcQueryHandler : IRequestHandler<GetAllDlcQueryRequest, List<GetAllDlcQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllDlcQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetAllDlcQueryResponse>> Handle(GetAllDlcQueryRequest request, CancellationToken cancellationToken)
        {
            var dlcList = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAllAsync(
                include: query => query.Include(d => d.Game),
                enableTracking: false,
                includeDeleted: request.IncludeDeleted
            );

            return _mapper.Map<List<GetAllDlcQueryResponse>>(dlcList);
        }
    }
}
