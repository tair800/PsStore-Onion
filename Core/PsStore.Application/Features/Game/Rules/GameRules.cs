using PsStore.Application.Features.Game.Exceptions;
using PsStore.Application.Interfaces.UnitOfWorks;
using PsStore.Domain.Entities;

namespace PsStore.Application.Features.Game.Rules
{
    public class GameRules
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameRules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task GameTitleMustBeUnique(string title)
        {
            bool exists = await _unitOfWork.GetReadRepository<Domain.Entities.Game>()
                .AnyAsync(g => g.Title == title);

            if (exists)
                throw new GameTitleAlreadyExistsException(title);
        }

        public async Task PlatformMustExist(int platformId)
        {
            bool exists = Enum.IsDefined(typeof(Platform), platformId);

            if (!exists)
                throw new PlatformNotFoundException(platformId);
        }

        public async Task CategoryMustExist(int categoryId)
        {
            bool exists = await _unitOfWork.GetReadRepository<Domain.Entities.Category>()
                .AnyAsync(c => c.Id == categoryId);

            if (!exists)
                throw new CategoryNotFoundException(categoryId);
        }
    }
}
