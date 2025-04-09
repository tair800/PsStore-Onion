using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PsStore.Application.Features.Auth.Queries.Get;

namespace PsStore.Application.Features.Auth.Queries
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQueryRequest, Result<GetUserQueryResponse>>
    {
        private readonly UserManager<Domain.Entities.User> _userManager;
        private readonly ILogger<GetUserQueryHandler> _logger;

        public GetUserQueryHandler(UserManager<Domain.Entities.User> userManager, ILogger<GetUserQueryHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<GetUserQueryResponse>> Handle(GetUserQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null)
            {
                _logger.LogWarning("User not found with Id: {Id}", request.Id);
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
