using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Category.Commands.UpdateCategory;
using PsStore.Application.Features.Category.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class UpdateCategoryCommandHandler : BaseHandler, IRequestHandler<UpdateCategoryCommandRequest, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CategoryRules _categoryRules;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UpdateCategoryCommandHandler> logger,
        CategoryRules categoryRules)
        : base(mapper, unitOfWork, httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _categoryRules = categoryRules;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<Unit>> Handle(UpdateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating category with ID {CategoryId}", request.Id);

        var categoryCheckResult = await _categoryRules.CategoryMustExist(request.Id);
        if (!categoryCheckResult.IsSuccess)
        {
            return Result<Unit>.Failure(categoryCheckResult.Error, categoryCheckResult.StatusCode, categoryCheckResult.ErrorCode);
        }

        var nameCheckResult = await _categoryRules.CategoryNameMustBeUnique(request.Name);
        if (!nameCheckResult.IsSuccess)
        {
            return Result<Unit>.Failure(nameCheckResult.Error, nameCheckResult.StatusCode, nameCheckResult.ErrorCode);
        }

        var category = await _unitOfWork.GetReadRepository<Category>().GetAsync(c => c.Id == request.Id, enableTracking: true);

        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found.", request.Id);
            return Result<Unit>.Failure("Category not found.", StatusCodes.Status404NotFound, "CATEGORY_NOT_FOUND");
        }

        _mapper.Map(request, category);
        category.UpdatedDate = DateTime.UtcNow;

        try
        {
            await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully updated category with ID {CategoryId}", request.Id);

            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating category with ID {CategoryId}", request.Id);
            return Result<Unit>.Failure("Failed to update category.", StatusCodes.Status500InternalServerError, "CATEGORY_UPDATE_FAILED");
        }
    }
}
