using FluentValidation;
using PsStore.Application.Features.Game.Commands;

public class UpdateGameCommandValidator : AbstractValidator<UpdateGameCommandRequest>
{
    public UpdateGameCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Game ID must be greater than zero.");

        When(x => x.Title is not null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Game title cannot be empty.")
                .MinimumLength(3).WithMessage("Game title must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("Game title cannot exceed 100 characters.");
        });

        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Game description cannot exceed 500 characters.");
        });

        When(x => x.Price is not null, () =>
        {
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be a positive number.");
        });

        When(x => x.SalePrice is not null, () =>
        {
            RuleFor(x => x.SalePrice)
                .GreaterThanOrEqualTo(0).WithMessage("Sale price must be a positive number.");
        });

        When(x => x.ImgUrl is not null, () =>
        {
            RuleFor(x => x.ImgUrl)
                .MaximumLength(255).WithMessage("Image URL cannot exceed 255 characters.");
        });

        //When(x => x.CategoryId is not null, () =>
        //{
        //    RuleFor(x => x.CategoryId)
        //        .GreaterThan(0).WithMessage("Category ID must be greater than zero.");
        //});

        When(x => x.PlatformId is not null, () =>
        {
            RuleFor(x => x.PlatformId)
                .GreaterThan(0).WithMessage("Platform ID must be greater than zero.");
        });
    }
}
