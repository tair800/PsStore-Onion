using FluentValidation;
using PsStore.Application.Features.Basket.Commands.AddToBasket;

namespace PsStore.Application.Features.Basket.Commands.AddBasket
{
    public class AddToBasketCommandValidator : AbstractValidator<AddToBasketCommandRequest>
    {
        public AddToBasketCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.GameId).GreaterThan(0).WithMessage("GameId must be a valid positive integer.");
        }
    }
}
