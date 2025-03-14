using FluentValidation;

namespace PsStore.Application.Features.Category.Commands.RestoreCategory
{
    public class RestoreCategoryCommandValidator : AbstractValidator<RestoreCategoryCommandRequest>
    {
        public RestoreCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid category ID.");
        }
    }
}
