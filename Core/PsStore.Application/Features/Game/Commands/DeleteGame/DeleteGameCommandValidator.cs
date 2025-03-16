using FluentValidation;
using PsStore.Application.Features.Game.Commands;

public class DeleteGameCommandValidator : AbstractValidator<DeleteGameCommandRequest>
{
    public DeleteGameCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Game ID must be greater than zero.");
    }
}
