using MediatR;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Queries.GetCategoriesWithGames
{
    public class GetCategoriesWithGamesQueryHandler : IRequestHandler<GetCategoriesWithGamesQueryRequest, List<GetCategoriesWithGamesQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoriesWithGamesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetCategoriesWithGamesQueryResponse>> Handle(GetCategoriesWithGamesQueryRequest request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.GetReadRepository<Domain.Entities.Category>().GetAllAsync(
                include: query => query.Include(c => c.Games),
                enableTracking: false
            );

            return _mapper.Map<List<GetCategoriesWithGamesQueryResponse>>(categories);
        }
    }
}
