using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Category.Exceptions;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : BaseHandler, IRequestHandler<UpdateCategoryCommandRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;

        public UpdateCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<UpdateCategoryCommandHandler> logger)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateCategoryCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating category with ID {CategoryId}", request.Id);

            var category = await _unitOfWork.GetReadRepository<Domain.Entities.Category>().GetAsync(
                predicate: c => c.Id == request.Id,
                include: q => q.Include(c => c.Games),
                enableTracking: true
            );

            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found.", request.Id);
                throw new CategoryNotFoundException(request.Id);
            }

            _mapper.Map(request, category);
            category.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.GetWriteRepository<Domain.Entities.Category>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully updated category with ID {CategoryId}", request.Id);

            return Unit.Value;
        }
    }
}
