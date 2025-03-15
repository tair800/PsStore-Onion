using MediatR;
using PsStore.Application.Features.Category.Exceptions;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Queries.GetCategoryById
{

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQueryRequest, GetCategoryByIdQueryResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetCategoryByIdQueryResponse> Handle(GetCategoryByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.GetReadRepository<Domain.Entities.Category>()
                .GetAsync(c => c.Id == request.Id);

            if (category == null)
                throw new CategoryNotFoundException(request.Id);

            return _mapper.Map<GetCategoryByIdQueryResponse>(category);
        }
    }
}
