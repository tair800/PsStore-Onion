using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Auth.RevokeAll
{
    public class RevokeAllCommandHandler : BaseHandler, IRequestHandler<RevokeAllCommandRequest, Result<Unit>>
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<RevokeAllCommandHandler> logger;

        public RevokeAllCommandHandler(
            UserManager<User> userManager,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RevokeAllCommandHandler> logger) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<Result<Unit>> Handle(RevokeAllCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Revoking refresh tokens for all users.");

                List<User> users = await userManager.Users.ToListAsync(cancellationToken);
                foreach (User user in users)
                {
                    user.RefreshToken = null;
                    IdentityResult result = await userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        logger.LogWarning("Failed to update refresh token for user {UserId}.", user.Id);
                        return Result<Unit>.Failure("Failed to revoke refresh token for some users.", StatusCodes.Status500InternalServerError, "USER_UPDATE_FAILED");
                    }
                }

                logger.LogInformation("Successfully revoked refresh tokens for all users.");
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while revoking refresh tokens for all users.");
                return Result<Unit>.Failure("An unexpected error occurred while processing the revoke all request.", StatusCodes.Status500InternalServerError, "INTERNAL_ERROR");
            }
        }
    }
}
