using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Exceptions;
using PsStore.Application.Features.Auth.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.Token;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace PsStore.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : BaseHandler, IRequestHandler<LoginCommandRequest, LoginCommandResponse>
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly ITokenService tokenService;
        private readonly AuthRules authRules;
        private readonly ILogger<LoginCommandHandler> logger;

        public LoginCommandHandler(
            UserManager<User> userManager,
            IConfiguration configuration,
            ITokenService tokenService,
            AuthRules authRules,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<LoginCommandHandler> logger)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.tokenService = tokenService;
            this.authRules = authRules;
            this.logger = logger;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Login attempt for email: {Email}", request.Email);

                User user = await userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    logger.LogWarning("Invalid login attempt: User not found for email: {Email}", request.Email);
                    throw new InvalidLoginException(request.Email);
                }

                bool checkPassword = await userManager.CheckPasswordAsync(user, request.Password);
                if (!checkPassword)
                {
                    logger.LogWarning("Invalid login attempt: Incorrect password for email: {Email}", request.Email);
                    throw new InvalidLoginException(request.Email);
                }

                IList<string> roles = await userManager.GetRolesAsync(user);
                JwtSecurityToken token = await tokenService.CreateToken(user, roles);
                string refreshToken = tokenService.GenerateRefreshToken();

                _ = int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpireTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    logger.LogError("Failed to update user {UserId} during login.", user.Id);
                    throw new UserUpdateFailedException(user.Id.ToString());
                }

                await userManager.UpdateSecurityStampAsync(user);

                string generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
                await userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", generatedToken);

                logger.LogInformation("User {UserId} logged in successfully.", user.Id);

                return new LoginCommandResponse
                {
                    Token = generatedToken,
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                };
            }
            catch (InvalidLoginException ex)
            {
                logger.LogWarning(ex, "Login failed due to invalid credentials.");
                throw;
            }
            catch (UserUpdateFailedException ex)
            {
                logger.LogError(ex, "Login failed due to user update failure.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the login request.");
                throw new Exception("An unexpected error occurred while processing your login request.");
            }
        }
    }
}
