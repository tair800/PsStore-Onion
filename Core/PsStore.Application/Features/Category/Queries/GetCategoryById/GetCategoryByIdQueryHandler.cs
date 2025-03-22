using MediatR;
using Microsoft.AspNetCore.Http;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQueryRequest, Result<GetCategoryByIdQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetCategoryByIdQueryResponse>> Handle(GetCategoryByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.GetReadRepository<Domain.Entities.Category>()
                .GetAsync(c => c.Id == request.Id);

            if (category == null)
            {
                return Result<GetCategoryByIdQueryResponse>.Failure("Category not found.", StatusCodes.Status404NotFound, "CATEGORY_NOT_FOUND");
            }

            var response = _mapper.Map<GetCategoryByIdQueryResponse>(category);

            return Result<GetCategoryByIdQueryResponse>.Success(response);
        }
    }
}
