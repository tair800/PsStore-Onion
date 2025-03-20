using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Exceptions;
using PsStore.Application.Features.Auth.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Auth.Revoke
{
    public class RevokeCommandHandler : BaseHandler, IRequestHandler<RevokeCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;
        private readonly AuthRules authRules;
        private readonly ILogger<RevokeCommandHandler> logger;

        public RevokeCommandHandler(
            UserManager<User> userManager,
            AuthRules authRules,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RevokeCommandHandler> logger) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.userManager = userManager;
            this.authRules = authRules;
            this.logger = logger;
        }

        public async Task<Unit> Handle(RevokeCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Revoking refresh token for email: {Email}", request.Email);

                User user = await userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    logger.LogWarning("User not found for email: {Email}", request.Email);
                    throw new UserNotFoundException(request.Email);
                }

                await authRules.UserShouldNotBeExist(user);

                user.RefreshToken = null;

                IdentityResult result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to update user {UserId} during revoke operation.", user.Id);
                    throw new UserUpdateFailedException(user.Id.ToString());
                }

                logger.LogInformation("Successfully revoked refresh token for user {UserId}.", user.Id);

                return Unit.Value;
            }
            catch (UserNotFoundException ex)
            {
                logger.LogWarning(ex, "Revocation failed due to user not found.");
                throw;
            }
            catch (UserUpdateFailedException ex)
            {
                logger.LogError(ex, "Revocation failed during user update.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while processing the revoke request.");
                throw new Exception("An unexpected error occurred while processing your request.");
            }
        }
    }
}
