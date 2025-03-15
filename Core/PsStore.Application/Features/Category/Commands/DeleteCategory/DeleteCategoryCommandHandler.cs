using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Category.Commands.DeleteCategory;
using PsStore.Application.Features.Category.Exceptions;
using PsStore.Application.Features.Category.Rules;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class DeleteCategoryCommandHandler : BaseHandler, IRequestHandler<DeleteCategoryCommandRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CategoryRules _categoryRules;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,
        ILogger<DeleteCategoryCommandHandler> logger, CategoryRules categoryRules)
        : base(null, unitOfWork, httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _categoryRules = categoryRules;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to delete category with ID {CategoryId}", request.Id);

        await _categoryRules.CategoryMustExist(request.Id);

        var category = await _unitOfWork.GetReadRepository<Category>().GetAsync(
            c => c.Id == request.Id, enableTracking: true
        );

        if (category.IsDeleted)
        {
            _logger.LogWarning("Category with ID {CategoryId} is already deleted.", request.Id);
            throw new CategoryAlreadyDeletedException(request.Id);
        }

        if (category.Games != null && category.Games.Any())
        {
            _logger.LogWarning("Category with ID {CategoryId} contains games and cannot be deleted.", request.Id);
            throw new CategoryCannotBeDeletedException(request.Id);
        }


        try
        {
            category.IsDeleted = true;
            category.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully deleted category with ID {CategoryId}", request.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete category with ID {CategoryId}", request.Id);
            throw new CategoryDeleteFailedException(request.Id);
        }

        return Unit.Value;
    }
}
