using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Category.Commands;
using PsStore.Application.Features.Category.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class CreateCategoryCommandHandler : BaseHandler, IRequestHandler<CreateCategoryCommandRequest, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CategoryRules _categoryRules;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,
        CategoryRules categoryRules, ILogger<CreateCategoryCommandHandler> logger)
        : base(mapper, unitOfWork, httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _categoryRules = categoryRules;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting category creation process for name: {CategoryName}", request.Name);

        var categoryNameValidationResult = _categoryRules.CategoryNameMustBeValid(request.Name);
        if (!categoryNameValidationResult.IsSuccess)
        {
            return categoryNameValidationResult;
        }

        var uniqueNameValidationResult = await _categoryRules.CategoryNameMustBeUnique(request.Name);
        if (!uniqueNameValidationResult.IsSuccess)
        {
            return uniqueNameValidationResult;
        }

        var category = new Category(request.Name);

        try
        {
            await _unitOfWork.GetWriteRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully created category with ID {CategoryId}", category.Id);
            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating category with name {CategoryName}", request.Name);
            return Result<Unit>.Failure("Category creation failed.", StatusCodes.Status500InternalServerError, "CATEGORY_CREATION_FAILED");
        }
    }
}
