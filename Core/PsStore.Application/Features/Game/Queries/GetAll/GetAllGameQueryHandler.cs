using MediatR;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Game.Queries.GetAllGame
{
    public class GetAllGameQueryHandler : IRequestHandler<GetAllGameQueryRequest, List<GetAllGameQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllGameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetAllGameQueryResponse>> Handle(GetAllGameQueryRequest request, CancellationToken cancellationToken)
        {
            var games = await _unitOfWork.GetReadRepository<Domain.Entities.Game>().GetAllAsync(
                include: query => query
                    .Include(g => g.Category)
                    .Include(g => g.Dlcs),
                enableTracking: false,
                includeDeleted: request.IncludeDeleted
            );

            var response = _mapper.Map<List<GetAllGameQueryResponse>>(games);

            foreach (var game in response)
            {
                game.PlatformName = games.FirstOrDefault(g => g.Id == game.Id)?.Platform.ToString();
            }

            return response;
        }
    }
}
