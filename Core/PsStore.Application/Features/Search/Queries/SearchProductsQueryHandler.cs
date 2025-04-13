using MediatR;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Search.Queries
{
    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQueryRequest, Result<List<SearchResult>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<SearchResult>>> Handle(SearchProductsQueryRequest request, CancellationToken cancellationToken)
        {
            var keyword = request.Keyword?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Result<List<SearchResult>>.Success(new List<SearchResult>());
            }

            var games = await _unitOfWork.GetReadRepository<Domain.Entities.Game>()
                .Find(g => g.Title.ToLower().Contains(keyword))
                .ToListAsync(cancellationToken);

            var dlcs = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>()
                .Find(d => d.Name.ToLower().Contains(keyword))
                .ToListAsync(cancellationToken);

            var gameResults = games.Select(game =>
            {
                var dto = _mapper.Map<SearchResult>(game);
                dto.Type = "Game";
                return dto;
            });

            var dlcResults = dlcs.Select(dlc =>
            {
                var dto = _mapper.Map<SearchResult>(dlc);
                dto.Type = "DLC";
                return dto;
            });

            var combinedResults = gameResults.Concat(dlcResults).ToList();

            return Result<List<SearchResult>>.Success(combinedResults);
        }
    }
}
