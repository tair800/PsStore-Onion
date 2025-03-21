using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Queries.GetAllDlc
{
    public class GetAllDlcQueryHandler : IRequestHandler<GetAllDlcQueryRequest, Result<List<GetAllDlcQueryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllDlcQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllDlcQueryResponse>>> Handle(GetAllDlcQueryRequest request, CancellationToken cancellationToken)
        {
            var dlcList = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAllAsync(
                include: query => query
                    .Include(d => d.Game),
                enableTracking: false,
                includeDeleted: request.IncludeDeleted
            );

            if (!dlcList.Any())
            {
                return Result<List<GetAllDlcQueryResponse>>.Failure("No DLCs found.", StatusCodes.Status404NotFound, "DLC_NOT_FOUND");
            }

            var response = _mapper.Map<List<GetAllDlcQueryResponse>>(dlcList);

            foreach (var dlc in response)
            {
                var gameName = dlcList.FirstOrDefault(d => d.Id == dlc.Id)?.Game?.Title;
                if (gameName != null)
                {
                    dlc.GameTitle = gameName;
                }
            }

            return Result<List<GetAllDlcQueryResponse>>.Success(response);
        }
    }
}
