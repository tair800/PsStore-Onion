using MediatR;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQueryRequest, List<GetAllCategoriesQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetAllCategoriesQueryResponse>> Handle(GetAllCategoriesQueryRequest request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.GetReadRepository<Domain.Entities.Category>().GetAllAsync(
                enableTracking: false,
                includeDeleted: request.IncludeDeleted
            );

            return _mapper.Map<List<GetAllCategoriesQueryResponse>>(categories);
        }
    }
}
