using MediatR;
using Microsoft.AspNetCore.Http;
using PsStore.Application.Bases;
using PsStore.Application.Features.Game.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Game.Commands
{
    public class CreateGameCommandHandler : BaseHandler, IRequestHandler<CreateGameCommandRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly GameRules _gameRules;

        public CreateGameCommandHandler(GameRules gameRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _gameRules = gameRules;
        }

        public async Task<Unit> Handle(CreateGameCommandRequest request, CancellationToken cancellationToken)
        {
            var existingGame = await _unitOfWork.GetReadRepository<Domain.Entities.Game>()
                .AnyAsync(g => g.Title == request.Title);
            if (existingGame)
            {
                throw new InvalidOperationException("A game with this title already exists.");
            }

            if (!Enum.IsDefined(typeof(Platform), request.PlatformId))
            {
                throw new InvalidOperationException("Invalid PlatformId. The specified platform does not exist.");
            }
            var platformEnum = (Platform)request.PlatformId;

            var categoryExists = await _unitOfWork.GetReadRepository<Domain.Entities.Category>()
                .AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
            {
                throw new InvalidOperationException("Invalid CategoryId. The specified category does not exist.");
            }

            List<Domain.Entities.Dlc> dlcs = new();
            if (request.DlcIds.Any())
            {
                dlcs = (await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>()
                    .GetAllAsync(d => request.DlcIds.Contains(d.Id)))
                    .ToList();
            }

            List<Rating> ratings = new();
            if (request.RatingIds.Any())
            {
                ratings = (await _unitOfWork.GetReadRepository<Rating>()
                    .GetAllAsync(r => request.RatingIds.Contains(r.Id)))
                    .ToList();
            }

            Domain.Entities.Game game = new()
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                SalePrice = request.SalePrice ?? 0,
                ImgUrl = request.ImgUrl,
                CategoryId = request.CategoryId,
                Platform = platformEnum,
                Dlcs = dlcs,
                Ratings = ratings
            };

            await _unitOfWork.GetWriteRepository<Domain.Entities.Game>().AddAsync(game);
            await _unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
