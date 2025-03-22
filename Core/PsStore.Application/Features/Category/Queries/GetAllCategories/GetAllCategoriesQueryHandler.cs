using MediatR;
using Microsoft.AspNetCore.Http;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQueryRequest, Result<List<GetAllCategoriesQueryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllCategoriesQueryResponse>>> Handle(GetAllCategoriesQueryRequest request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.GetReadRepository<Domain.Entities.Category>().GetAllAsync(
                enableTracking: false,
                includeDeleted: request.IncludeDeleted
            );

            if (categories == null || !categories.Any())
            {
                return Result<List<GetAllCategoriesQueryResponse>>.Failure("No categories found.", StatusCodes.Status404NotFound, "CATEGORIES_NOT_FOUND");
            }

            var response = _mapper.Map<List<GetAllCategoriesQueryResponse>>(categories);

            return Result<List<GetAllCategoriesQueryResponse>>.Success(response);
        }
    }
}
