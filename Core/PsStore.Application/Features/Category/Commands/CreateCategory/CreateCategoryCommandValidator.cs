using FluentValidation;

namespace PsStore.Application.Features.Category.Commands
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommandRequest>
    {
        //public CreateCategoryCommandValidator()
        //{
        //    RuleFor(x => x.Name)
        //        .NotEmpty().WithMessage("Category name is required.")
        //        .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");
        //}
    }
}
