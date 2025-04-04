using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PsStore.Application.Features.Auth.Queries.Get;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Auth.Queries
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQueryRequest, Result<GetUserQueryResponse>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<GetUserQueryHandler> _logger;

        public GetUserQueryHandler(UserManager<User> userManager, ILogger<GetUserQueryHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<GetUserQueryResponse>> Handle(GetUserQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogWarning("User not found with email: {Email}", request.Email);
                return Result<GetUserQueryResponse>.Failure("User not found.", 404, "USER_NOT_FOUND");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var response = new GetUserQueryResponse
            {
                FullName = user.FullName,
                Email = user.Email,
                UserId = user.Id.ToString(),
                Roles = roles.ToList(),
                CreatedDate = user.CreatedDate
            };

            return Result<GetUserQueryResponse>.Success(response);
        }
    }
}
