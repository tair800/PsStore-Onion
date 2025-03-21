using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Game.Queries.GetGameById
{
    public class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQueryRequest, Result<GetGameByIdQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetGameByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetGameByIdQueryResponse>> Handle(GetGameByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var game = await _unitOfWork.GetReadRepository<Domain.Entities.Game>().GetAsync(
                g => g.Id == request.Id,
                include: query => query
                    .Include(g => g.Category)
                    .Include(g => g.Dlcs)
                    .Include(g => g.Ratings),
                enableTracking: false,
                includeDeleted: request.IncludeDeleted
            );

            if (game == null)
            {
                return Result<GetGameByIdQueryResponse>.Failure("Game not found.", StatusCodes.Status404NotFound, "GAME_NOT_FOUND");
            }

            var response = _mapper.Map<GetGameByIdQueryResponse>(game);

            response.PlatformName = game.Platform.ToString();

            return Result<GetGameByIdQueryResponse>.Success(response);
        }
    }
}
