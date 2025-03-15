using FluentValidation;
using PsStore.Application.Features.Dlc.Commands;

public class RestoreDlcCommandValidator : AbstractValidator<RestoreDlcCommandRequest>
{
    public RestoreDlcCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("DLC ID must be greater than zero.");
    }
}
