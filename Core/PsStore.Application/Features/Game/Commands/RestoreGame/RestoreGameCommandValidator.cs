using FluentValidation;
using PsStore.Application.Features.Game.Commands;

public class RestoreGameCommandValidator : AbstractValidator<RestoreGameCommandRequest>
{
    public RestoreGameCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Game ID must be greater than zero.");
    }
}
