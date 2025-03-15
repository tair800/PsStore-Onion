using FluentValidation;
using PsStore.Application.Features.Dlc.Commands;

public class DeleteDlcCommandValidator : AbstractValidator<DeleteDlcCommandRequest>
{
    public DeleteDlcCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("DLC ID must be greater than zero.");
    }
}
