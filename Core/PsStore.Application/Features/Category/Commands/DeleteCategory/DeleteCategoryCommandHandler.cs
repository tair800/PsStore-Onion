using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Category.Exceptions;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : BaseHandler, IRequestHandler<DeleteCategoryCommandRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<DeleteCategoryCommandHandler> logger)
            : base(null, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteCategoryCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete category with ID {CategoryId}", request.Id);

            var category = await _unitOfWork.GetReadRepository<Domain.Entities.Category>().GetAsync(
                predicate: c => c.Id == request.Id,
                enableTracking: true
            );

            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found.", request.Id);
                throw new CategoryNotFoundException(request.Id);
            }

            if (category.IsDeleted)
            {
                _logger.LogWarning("Category with ID {CategoryId} is already deleted.", request.Id);
                throw new CategoryAlreadyDeletedException(request.Id);
            }

            category.IsDeleted = true;
            category.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.GetWriteRepository<Domain.Entities.Category>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully deleted category with ID {CategoryId}", request.Id);

            return Unit.Value;
        }
    }
}
