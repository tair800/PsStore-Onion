using MediatR;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Features.Dlc.Queries.GetDlcById;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Game.Queries.GetDlcByGame
{
    public class GetDlcsByGameIdQueryHandler : IRequestHandler<GetDlcsByGameIdQueryRequest, Result<List<GetDlcByIdQueryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDlcsByGameIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetDlcByIdQueryResponse>>> Handle(GetDlcsByGameIdQueryRequest request, CancellationToken cancellationToken)
        {
            var dlcs = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>()
                .GetAllAsync(d => d.GameId == request.GameId, include: q => q.Include(d => d.Game), includeDeleted: request.IncludeDeleted);

            var response = _mapper.Map<List<GetDlcByIdQueryResponse>>(dlcs);

            return Result<List<GetDlcByIdQueryResponse>>.Success(response);
        }
    }
}
