using MediatR;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Queries.GetDlcById
{
    public class GetDlcByIdQueryHandler : IRequestHandler<GetDlcByIdQueryRequest, GetDlcByIdQueryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDlcByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetDlcByIdQueryResponse> Handle(GetDlcByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAsync(
                predicate: d => d.Id == request.Id,
                include: q => q.Include(d => d.Game),
                enableTracking: false,
                includeDeleted: request.IncludeDeleted
            );

            if (dlc is null)
                throw new KeyNotFoundException($"DLC with ID '{request.Id}' not found.");

            return _mapper.Map<GetDlcByIdQueryResponse>(dlc);
        }
    }
}
