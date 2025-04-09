using MediatR;
using Microsoft.AspNetCore.Http;
using PsStore.Application.Bases;

namespace PsStore.Application.Features.Auth.Rules
{
    public class AuthRules : BaseRules
    {
        public Task<Result<Unit>> UserShouldNotBeExist(Domain.Entities.User? user)
        {
            if (user is not null)
            {
                return Task.FromResult(Result<Unit>.Failure("User already exists.", StatusCodes.Status400BadRequest, "USER_ALREADY_EXISTS"));
            }
            return Task.FromResult(Result<Unit>.Success(Unit.Value));
        }

        public Task<Result<Unit>> EmailOrPasswordShouldntBeInvalidException(Domain.Entities.User user, bool checkPassword)
        {
            if (user is null || !checkPassword)
            {
                return Task.FromResult(Result<Unit>.Failure("Invalid email or password.", StatusCodes.Status400BadRequest, "INVALID_CREDENTIALS"));
            }
            return Task.FromResult(Result<Unit>.Success(Unit.Value));
        }

        public Task<Result<Unit>> RefreshTokenShouldNotBeExpired(DateTime? expiryDate)
        {
            if (expiryDate <= DateTime.Now)
            {
                return Task.FromResult(Result<Unit>.Failure("Refresh token expired.", StatusCodes.Status400BadRequest, "REFRESH_TOKEN_EXPIRED"));
            }
            return Task.FromResult(Result<Unit>.Success(Unit.Value));
        }
    }
}
