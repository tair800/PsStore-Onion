using FluentValidation;

namespace PsStore.Application.Features.Category.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommandRequest>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid category ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must be at most 100 characters.");

            RuleFor(x => x.GameIds)
                .Must(gameIds => gameIds == null || gameIds.All(id => id > 0))
                .WithMessage("All game IDs must be greater than zero.");
        }
    }
}
