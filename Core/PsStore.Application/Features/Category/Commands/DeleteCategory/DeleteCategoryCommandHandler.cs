using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PsStore.Application.Features.Category.Commands.DeleteCategory;
using PsStore.Application.Features.Category.Rules;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommandRequest, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CategoryRules _categoryRules;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DeleteCategoryCommandHandler> logger,
        CategoryRules categoryRules)
    {
        _unitOfWork = unitOfWork;
        _categoryRules = categoryRules;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to delete category with ID {CategoryId}", request.Id);

        // Retrieve the category including games (ensure we have the games loaded)
        var category = await _unitOfWork.GetReadRepository<Category>().GetAsync(
            c => c.Id == request.Id, enableTracking: true, include: q => q.Include(c => c.Games));

        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found.", request.Id);
            return Result<Unit>.Failure("Category not found.", StatusCodes.Status404NotFound, "CATEGORY_NOT_FOUND");
        }

        if (category.Games != null && category.Games.Any())
        {
            _logger.LogWarning("Category with ID {CategoryId} contains games and cannot be deleted.", request.Id);
            return Result<Unit>.Failure("Category contains games and cannot be deleted.", StatusCodes.Status400BadRequest, "CATEGORY_CONTAINS_GAMES");
        }

        category.IsDeleted = true;
        category.UpdatedDate = DateTime.UtcNow;

        try
        {
            await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully deleted category with ID {CategoryId}", request.Id);
            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete category with ID {CategoryId}", request.Id);
            return Result<Unit>.Failure("Failed to delete category.", StatusCodes.Status500InternalServerError, "CATEGORY_DELETE_FAILED");
        }
    }
}
