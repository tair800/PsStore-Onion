using FluentValidation;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class UpdateDlcCommandValidator : AbstractValidator<UpdateDlcCommandRequest>
    {
        public UpdateDlcCommandValidator()
        {
            RuleFor(d => d.Id)
                .GreaterThan(0).WithMessage("DLC ID must be greater than 0.");

            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");

            RuleFor(d => d.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(d => d.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            //RuleFor(d => d.SalePrice)
            //    .GreaterThanOrEqualTo(0).WithMessage("Sale Price must be non-negative.");

            //RuleFor(d => d.ImgUrl)
            //    .NotEmpty().WithMessage("Image URL is required.")
            //    .Matches(@"^(http|https)://").WithMessage("Image URL must be a valid URL.");

            RuleFor(d => d.GameId)
                .GreaterThan(0).WithMessage("Game ID must be greater than 0.");
        }
    }
}
