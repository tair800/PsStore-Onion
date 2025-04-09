using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;  // Add this for ToListAsync
using PsStore.Application.Features.Auth.Queries.GetAll;

namespace PsStore.Application.Features.Auth.Queries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQueryRequest, Result<List<GetAllUsersQueryResponse>>>
    {
        private readonly UserManager<Domain.Entities.User> _userManager;

        public GetAllUsersQueryHandler(UserManager<Domain.Entities.User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<List<GetAllUsersQueryResponse>>> Handle(GetAllUsersQueryRequest request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);

            var userList = new List<GetAllUsersQueryResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userList.Add(new GetAllUsersQueryResponse
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    UserId = user.Id.ToString(),
                    Roles = roles.ToList(),
                    CreatedDate = user.CreatedDate
                });
            }

            return Result<List<GetAllUsersQueryResponse>>.Success(userList);
        }
    }
}
