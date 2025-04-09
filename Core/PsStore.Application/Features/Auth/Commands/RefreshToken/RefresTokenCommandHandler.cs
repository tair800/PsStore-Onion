using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Auth.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.Token;
using PsStore.Application.Interfaces.UnitOfWorks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PsStore.Application.Features.Auth.Commands.RefreshToken
{
    public class RefresTokenCommandHandler : BaseHandler, IRequestHandler<RefresTokenCommandRequest, Result<RefresTokenCommandResponse>>
    {
        private readonly UserManager<Domain.Entities.User> userManager;
        private readonly AuthRules authRules;
        private readonly ITokenService tokenService;
        private readonly ILogger<RefresTokenCommandHandler> logger;

        public RefresTokenCommandHandler(
            UserManager<Domain.Entities.User> userManager,
            AuthRules authRules,
            ITokenService tokenService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RefresTokenCommandHandler> logger)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.userManager = userManager;
            this.authRules = authRules;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        public async Task<Result<RefresTokenCommandResponse>> Handle(RefresTokenCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Processing refresh token request for access token: {AccessToken}", request.AccessToken);

                ClaimsPrincipal? principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
                string email = principal?.FindFirstValue(ClaimTypes.Email);

                if (email == null)
                {
                    logger.LogWarning("Refresh token request failed: Email claim not found in expired token.");
                    return Result<RefresTokenCommandResponse>.Failure("Invalid token: Email claim missing.", StatusCodes.Status400BadRequest, "INVALID_TOKEN");
                }

                Domain.Entities.User? user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    logger.LogWarning("Refresh token request failed: User not found for email: {Email}", email);
                    return Result<RefresTokenCommandResponse>.Failure($"User not found for email: {email}.", StatusCodes.Status404NotFound, "USER_NOT_FOUND");
                }

                logger.LogInformation("User {UserId} found. Validating refresh token expiration.", user.Id);
                await authRules.RefreshTokenShouldNotBeExpired(user.RefreshTokenExpireTime);

                JwtSecurityToken newAccessToken = await tokenService.CreateToken(user, await userManager.GetRolesAsync(user));
                string newRefreshToken = tokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                await userManager.UpdateAsync(user);

                logger.LogInformation("Successfully refreshed tokens for user {UserId}", user.Id);

                return Result<RefresTokenCommandResponse>.Success(new RefresTokenCommandResponse
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the refresh token request.");
                return Result<RefresTokenCommandResponse>.Failure("An unexpected error occurred while processing the request.", StatusCodes.Status500InternalServerError, "INTERNAL_ERROR");
            }
        }
    }
}
