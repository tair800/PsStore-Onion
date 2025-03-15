using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Category.Commands;
using PsStore.Application.Features.Category.Exceptions;
using PsStore.Application.Features.Category.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class CreateCategoryCommandHandler : BaseHandler, IRequestHandler<CreateCategoryCommandRequest, Unit>
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

    public async Task<Unit> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting category creation process for name: {CategoryName}", request.Name);

        // Validate the category name
        _categoryRules.CategoryNameMustBeValid(request.Name);

        // Ensure uniqueness
        await _categoryRules.CategoryNameMustBeUnique(request.Name);

        Category category = new(request.Name);

        try
        {
            // Add category to the database
            await _unitOfWork.GetWriteRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Successfully created category with ID {CategoryId}", category.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating category with name {CategoryName}", request.Name);
            throw new CategoryCreationFailedException(request.Name);
        }

        return Unit.Value;
    }
}
