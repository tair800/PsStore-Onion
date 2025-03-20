using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Auth.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.Token;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace PsStore.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : BaseHandler, IRequestHandler<LoginCommandRequest, Result<LoginCommandResponse>>
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

        public async Task<Result<LoginCommandResponse>> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Login attempt for email: {Email}", request.Email);

            User user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                logger.LogWarning("Invalid login attempt: User not found for email: {Email}", request.Email);
                return Result<LoginCommandResponse>.Failure("Invalid email or password.", StatusCodes.Status400BadRequest, "INVALID_CREDENTIALS");
            }

            bool checkPassword = await userManager.CheckPasswordAsync(user, request.Password);
            if (!checkPassword)
            {
                logger.LogWarning("Invalid login attempt: Incorrect password for email: {Email}", request.Email);
                return Result<LoginCommandResponse>.Failure("Invalid email or password.", StatusCodes.Status400BadRequest, "INVALID_CREDENTIALS");
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
                return Result<LoginCommandResponse>.Failure($"Failed to update user {user.Id}.", StatusCodes.Status500InternalServerError, "USER_UPDATE_FAILED");
            }

            await userManager.UpdateSecurityStampAsync(user);

            string generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
            await userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", generatedToken);

            logger.LogInformation("User {UserId} logged in successfully.", user.Id);

            return Result<LoginCommandResponse>.Success(new LoginCommandResponse
            {
                Token = generatedToken,
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }
    }
}
