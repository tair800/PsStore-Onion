using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Game.Queries.GetAllGame
{
    public class GetAllGameQueryHandler : IRequestHandler<GetAllGameQueryRequest, Result<List<GetAllGameQueryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllGameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllGameQueryResponse>>> Handle(GetAllGameQueryRequest request, CancellationToken cancellationToken)
        {
            // Retrieve the games with their related data (Category, DLCs)
            var games = await _unitOfWork.GetReadRepository<Domain.Entities.Game>().GetAllAsync(
                include: query => query
                    .Include(g => g.Category)
                    .Include(g => g.Dlcs),
                enableTracking: false,
                includeDeleted: request.IncludeDeleted
            );

            if (!games.Any())
            {
                return Result<List<GetAllGameQueryResponse>>.Failure("No games found.", StatusCodes.Status404NotFound, "GAMES_NOT_FOUND");
            }

            // Map the game entities to response DTOs
            var response = _mapper.Map<List<GetAllGameQueryResponse>>(games);

            // Set PlatformName in the response
            foreach (var game in response)
            {
                var platform = games.FirstOrDefault(g => g.Id == game.Id)?.Platform.ToString();
                if (platform != null)
                {
                    game.PlatformName = platform;
                }
            }

            // Return the successful result
            return Result<List<GetAllGameQueryResponse>>.Success(response);
        }
    }
}
