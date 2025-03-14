using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Category.Exceptions;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Category.Commands.RestoreCategory
{
    public class RestoreCategoryCommandHandler : BaseHandler, IRequestHandler<RestoreCategoryCommandRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RestoreCategoryCommandHandler> _logger;

        public RestoreCategoryCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ILogger<RestoreCategoryCommandHandler> logger)
            : base(null, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(RestoreCategoryCommandRequest request, CancellationToken cancellationToken)
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
                throw new CategoryNotFoundException(request.Id);
            }

            if (!category.IsDeleted)
            {
                _logger.LogWarning("Category with ID {CategoryId} is already active.", request.Id);
                throw new CategoryAlreadyActiveException(request.Id);
            }

            await _unitOfWork.GetWriteRepository<Domain.Entities.Category>().RestoreAsync(category);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully restored category with ID {CategoryId}", request.Id);

            return Unit.Value;
        }
    }
}
