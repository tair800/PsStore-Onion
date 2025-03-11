using FluentValidation;

namespace PsStore.Application.Features.Game.Commands
{
    public class CreateGameCommandValidator : AbstractValidator<CreateGameCommandRequest>
    {
        public CreateGameCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot be longer than 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description cannot be longer than 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");


            RuleFor(x => x.ImgUrl)
                .NotEmpty().WithMessage("Image URL is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("Invalid image URL format.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Invalid CategoryId.");

            RuleFor(x => x.PlatformId)
                .NotNull().WithMessage("Platform is required.");
        }
    }
}
