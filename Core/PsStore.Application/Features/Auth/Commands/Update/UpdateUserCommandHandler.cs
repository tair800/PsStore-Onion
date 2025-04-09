using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PsStore.Application.Bases;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.User.Commands.Update
{
    public class UpdateUserCommandHandler : BaseHandler, IRequestHandler<UpdateUserCommandRequest, Result<Unit>>
    {
        private readonly UserManager<Domain.Entities.User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(
            UserManager<Domain.Entities.User> userManager,
            RoleManager<Role> roleManager,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateUserCommandHandler> logger) : base(mapper, unitOfWork, httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(UpdateUserCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update User with ID {UserId}", request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", request.UserId);
                return Result<Unit>.Failure("User not found.", StatusCodes.Status404NotFound, "USER_NOT_FOUND");
            }

            // Only update FullName if provided
            if (!string.IsNullOrEmpty(request.FullName))
            {
                user.FullName = request.FullName;
                _logger.LogInformation("Updated full name for User {UserId}", request.UserId);
            }

            // Only update Email if provided and if it's different from the current one
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                user.Email = request.Email;
                user.UserName = request.Email;  // Also update UserName as it's often tied to Email
                _logger.LogInformation("Updated email for User {UserId}", request.UserId);
            }

            // Update Roles if provided
            if (request.Roles != null && request.Roles.Any())
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToAdd = request.Roles.Except(currentRoles);
                var rolesToRemove = currentRoles.Except(request.Roles);

                if (rolesToAdd.Any())
                {
                    await _userManager.AddToRolesAsync(user, rolesToAdd);
                    _logger.LogInformation("Added roles to User {UserId}: {Roles}", request.UserId, string.Join(", ", rolesToAdd));
                }

                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    _logger.LogInformation("Removed roles from User {UserId}: {Roles}", request.UserId, string.Join(", ", rolesToRemove));
                }
            }
            else
            {
                _logger.LogInformation("Roles not provided, skipping role update.");
            }

            try
            {
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    _logger.LogInformation("Successfully updated User with ID {UserId}", request.UserId);
                    return Result<Unit>.Success(Unit.Value);
                }
                else
                {
                    _logger.LogWarning("User update failed for UserId {UserId}", request.UserId);
                    return Result<Unit>.Failure("User update failed.", StatusCodes.Status500InternalServerError, "USER_UPDATE_FAILED");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during update for User {UserId}", request.UserId);
                return Result<Unit>.Failure("An unexpected error occurred during update.", StatusCodes.Status500InternalServerError, "UPDATE_ERROR");
            }
        }
    }
}
