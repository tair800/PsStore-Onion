using FluentValidation;

namespace PsStore.Application.Features.Auth.Commands.RefreshToken
{
    public class RefresTokenCommandValidator : AbstractValidator<RefresTokenCommandRequest>
    {
        public RefresTokenCommandValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
