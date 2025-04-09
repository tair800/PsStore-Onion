using FluentValidation;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommandRequest>
{
    public UpdateUserCommandValidator()
    {
        //RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId cannot be empty.");

        When(x => !string.IsNullOrEmpty(x.FullName), () =>
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("FullName cannot be empty.");

        });

        When(x => !string.IsNullOrEmpty(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.");
        });

        When(x => x.Roles != null && x.Roles.Any(), () =>
        {
            RuleForEach(x => x.Roles).NotEmpty().WithMessage("Role cannot be empty.");
        });
    }
}
