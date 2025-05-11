using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Features.Auth.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.Services;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;
using System.Net;

namespace PsStore.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : BaseHandler, IRequestHandler<RegisterCommandRequest, Result<Unit>>
    {
        private readonly AuthRules authRules;
        private readonly UserManager<Domain.Entities.User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly ILogger<RegisterCommandHandler> logger;
        private readonly IEmailService emailService;

        public RegisterCommandHandler(
            AuthRules authRules,
            UserManager<Domain.Entities.User> userManager,
            RoleManager<Role> roleManager,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RegisterCommandHandler> logger,
            IEmailService emailService) : base(mapper, unitOfWork, httpContextAccessor)
        {
            this.authRules = authRules;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
            this.emailService = emailService;
        }

        public async Task<Result<Unit>> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Register attempt for email: {Email}", request.Email);

                var existingUser = await userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    logger.LogWarning("Registration failed: User already exists for email: {Email}", request.Email);
                    return Result<Unit>.Failure("User with this email already exists.", StatusCodes.Status400BadRequest, "USER_ALREADY_EXISTS");
                }

                Domain.Entities.User user = mapper.Map<Domain.Entities.User>(request);
                user.UserName = request.Email;
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.CreatedDate = DateTime.UtcNow;

                IdentityResult result = await userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    if (!await roleManager.RoleExistsAsync("user"))
                    {
                        logger.LogInformation("Creating 'User' role because it doesn't exist.");
                        await roleManager.CreateAsync(new Role
                        {
                            Id = Guid.NewGuid(),
                            Name = "User",
                            NormalizedName = "USER",
                            ConcurrencyStamp = Guid.NewGuid().ToString(),
                        });
                    }

                    await userManager.AddToRoleAsync(user, "user");

                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = WebUtility.UrlEncode(token);
                    var confirmUrl = $"http://localhost:5000/api/auth/confirm-email?token={encodedToken}&email={user.Email}";

                    var subject = "Confirm Your Email";
                    var body = $"<p>Click the link to verify your account:</p><p><a href=\"{confirmUrl}\">{confirmUrl}</a></p>";
                    await emailService.SendEmailAsync(user.Email, subject, body);

                    logger.LogInformation("User {UserId} registered and verification email sent.", user.Id);
                    return Result<Unit>.Success(Unit.Value);
                }
                else
                {
                    logger.LogWarning("User registration failed for email: {Email}", request.Email);
                    return Result<Unit>.Failure("User registration failed.", StatusCodes.Status500InternalServerError, "USER_REGISTRATION_FAILED");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred during registration for email: {Email}", request.Email);
                return Result<Unit>.Failure("An unexpected error occurred during registration.", StatusCodes.Status500InternalServerError, "REGISTRATION_ERROR");
            }
        }
    }
}
