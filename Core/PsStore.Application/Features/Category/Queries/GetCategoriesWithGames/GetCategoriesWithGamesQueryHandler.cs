using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Queries.GetCategoriesWithGames
{
    public class GetCategoriesWithGamesQueryHandler : IRequestHandler<GetCategoriesWithGamesQueryRequest, Result<List<GetCategoriesWithGamesQueryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoriesWithGamesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetCategoriesWithGamesQueryResponse>>> Handle(GetCategoriesWithGamesQueryRequest request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.GetReadRepository<Domain.Entities.Category>().GetAllAsync(
                include: query => query.Include(c => c.Games),
                enableTracking: false
            );

            if (categories == null || !categories.Any())
            {
                return Result<List<GetCategoriesWithGamesQueryResponse>>.Failure("No categories found.", StatusCodes.Status404NotFound, "CATEGORIES_NOT_FOUND");
            }

            var response = _mapper.Map<List<GetCategoriesWithGamesQueryResponse>>(categories);

            return Result<List<GetCategoriesWithGamesQueryResponse>>.Success(response);
        }
    }
}
