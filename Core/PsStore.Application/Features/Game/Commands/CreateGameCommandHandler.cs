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
            IList<Domain.Entities.Game> games = await _unitOfWork.GetReadRepository<Domain.Entities.Game>().GetAllAsync();

            await _gameRules.GameTitleMustNotBeSame(games, request.Title);

            if (!Enum.IsDefined(typeof(Platform), request.PlatformId))
            {
                throw new Exception("Invalid PlatformId. The specified platform does not exist.");
            }

            var category = await _unitOfWork.GetReadRepository<Domain.Entities.Category>()
                .GetAsync(c => c.Id == request.CategoryId);
            if (category == null)
            {
                throw new Exception("Invalid CategoryId. The specified category does not exist.");
            }

            Domain.Entities.Game game = new()
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                SalePrice = request.SalePrice ?? 0,
                ImgUrl = request.ImgUrl,
                Category = category,
                Platform = (Platform)request.PlatformId,
                Dlcs = new List<Dlc>(),
                Ratings = new List<Rating>()
            };

            await _unitOfWork.GetWriteRepository<Domain.Entities.Game>().AddAsync(game);

            if (await _unitOfWork.SaveAsync() > 0)
            {
                var dlcs = await _unitOfWork.GetReadRepository<Dlc>().GetAllAsync(d => d.GameId == game.Id);
                if (dlcs.Any())
                {
                    foreach (var dlc in dlcs)
                    {
                        game.Dlcs.Add(dlc);
                    }
                }

                var ratings = await _unitOfWork.GetReadRepository<Rating>().GetAllAsync(r => r.GameId == game.Id);
                if (ratings.Any())
                {
                    foreach (var rating in ratings)
                    {
                        game.Ratings.Add(rating);
                    }
                }

                await _unitOfWork.SaveAsync();
            }

            return Unit.Value;
        }
    }
}
