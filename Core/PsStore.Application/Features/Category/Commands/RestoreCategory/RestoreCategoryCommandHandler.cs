using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Commands.RestoreCategory
{
    public class RestoreCategoryCommandHandler : BaseHandler, IRequestHandler<RestoreCategoryCommandRequest, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RestoreCategoryCommandHandler> _logger;

        public RestoreCategoryCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<RestoreCategoryCommandHandler> logger)
            : base(null, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(RestoreCategoryCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to restore category with ID {CategoryId}", request.Id);

            var category = await _unitOfWork.GetReadRepository<Domain.Entities.Category>().GetAsync(
                predicate: c => c.Id == request.Id,
                enableTracking: true,
                includeDeleted: true
            );

            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found.", request.Id);
                return Result<Unit>.Failure("Category not found.", StatusCodes.Status404NotFound, "CATEGORY_NOT_FOUND");
            }

            if (!category.IsDeleted)
            {
                _logger.LogWarning("Category with ID {CategoryId} is already active.", request.Id);
                return Result<Unit>.Failure("Category is already active.", StatusCodes.Status409Conflict, "CATEGORY_ALREADY_ACTIVE");
            }

            try
            {
                await _unitOfWork.GetWriteRepository<Domain.Entities.Category>().RestoreAsync(category);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Successfully restored category with ID {CategoryId}", request.Id);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore category with ID {CategoryId}", request.Id);
                return Result<Unit>.Failure("Failed to restore category.", StatusCodes.Status500InternalServerError, "CATEGORY_RESTORE_FAILED");
            }
        }
    }
}
