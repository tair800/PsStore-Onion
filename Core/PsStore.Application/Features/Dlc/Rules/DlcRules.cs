using PsStore.Application.Features.Dlc.Exceptions;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Rules
{
    public class DlcRules
    {
        private readonly IUnitOfWork _unitOfWork;

        public DlcRules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task GameMustExist(int gameId)
        {
            var gameExists = await _unitOfWork.GetReadRepository<Domain.Entities.Game>()
                .AnyAsync(g => g.Id == gameId);

            if (!gameExists)
            {
                throw new DlcNotFoundException(gameId);
            }
        }

        public async Task DlcNameMustBeUnique(int gameId, string dlcName)
        {
            var existingDlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>()
                .AnyAsync(d => d.Name == dlcName && d.GameId == gameId);

            if (existingDlc)
            {
                throw new DlcAlreadyExistsException(dlcName, gameId);
            }
        }

        public void PriceMustBeValid(double price)
        {
            if (price <= 0)
            {
                throw new ArgumentException("DLC price must be greater than zero.");
            }
        }
    }
}
