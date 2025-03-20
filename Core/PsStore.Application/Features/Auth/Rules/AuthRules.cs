using PsStore.Application.Bases;
using PsStore.Application.Features.Auth.Exceptions;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Auth.Rules
{
    public class AuthRules : BaseRules
    {
        public Task UserShouldNotBeExist(User? user)
        {
            if (user is not null) throw new UserAlreadyExistException();
            return Task.CompletedTask;
        }

        public Task EmailOrPasswordShouldntBeInvalidException(User user, bool checkPassword)
        {
            if (user is null || !checkPassword) throw new EmailOrPasswordShouldntBeInvalidException();
            return Task.CompletedTask;
        }

        public Task RefreshTokenShouldNotBeExpired(DateTime? expiryDate)
        {
            if (expiryDate <= DateTime.Now) throw new RefreshTokenShouldNotBeExpiredException();
            return Task.CompletedTask;
        }
    }
}
