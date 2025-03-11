using MediatR;
using Microsoft.AspNetCore.Http;
using PsStore.Application.Bases;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Commands
{
    public class CreateCategoryCommandHandler : BaseHandler, IRequestHandler<CreateCategoryCommandRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
        {

            bool isDuplicate = await _unitOfWork
       .GetReadRepository<Domain.Entities.Category>()
       .AnyAsync(c => c.Name == request.Name);

            if (isDuplicate)
            {
                throw new InvalidOperationException("A category with this name already exists.");
            }

            // Create new category entity
            Domain.Entities.Category category = new(request.Name);

            // Add the category to the repository
            await _unitOfWork.GetWriteRepository<Domain.Entities.Category>().AddAsync(category);

            // Save changes
            await _unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
