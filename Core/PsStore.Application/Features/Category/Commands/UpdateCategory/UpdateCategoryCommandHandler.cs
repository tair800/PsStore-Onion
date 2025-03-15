using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Category.Commands.UpdateCategory;
using PsStore.Application.Features.Category.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

public class UpdateCategoryCommandHandler : BaseHandler, IRequestHandler<UpdateCategoryCommandRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CategoryRules _categoryRules;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,
        ILogger<UpdateCategoryCommandHandler> logger, CategoryRules categoryRules)
        : base(mapper, unitOfWork, httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _categoryRules = categoryRules;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating category with ID {CategoryId}", request.Id);

        await _categoryRules.CategoryMustExist(request.Id);

        await _categoryRules.CategoryNameMustBeUnique(request.Name);

        var category = await _unitOfWork.GetReadRepository<Category>().GetAsync(
            c => c.Id == request.Id, enableTracking: true
        );

        _mapper.Map(request, category);
        category.UpdatedDate = DateTime.UtcNow;

        await _unitOfWork.GetWriteRepository<Category>().UpdateAsync(category);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation("Successfully updated category with ID {CategoryId}", request.Id);

        return Unit.Value;
    }
}
