using FluentValidation;

namespace PsStore.Application.Features.Dlc.Commands.CreateDlc
{
    public class CreateDlcCommandValidator : AbstractValidator<CreateDlcCommandRequest>
    {
        public CreateDlcCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("DLC name is required.")
                .MaximumLength(100).WithMessage("DLC name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            //RuleFor(x => x.SalePrice)
            //    .GreaterThan(0).When(x => x.SalePrice.HasValue)
            //    .WithMessage("Sale price must be greater than zero.");

            //RuleFor(x => x.ImgUrl)
            //    .NotEmpty().WithMessage("Image URL is required.")
            //    .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("Invalid image URL format.");

            RuleFor(x => x.GameId)
                .GreaterThan(0).WithMessage("Invalid GameId.");
        }
    }
}
