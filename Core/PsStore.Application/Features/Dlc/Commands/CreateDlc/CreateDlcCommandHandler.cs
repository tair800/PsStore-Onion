using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PsStore.Application.Bases;
using PsStore.Application.Features.Dlc.Commands.CreateDlc;
using PsStore.Application.Features.Dlc.Rules;
using PsStore.Application.Interfaces.AutoMapper;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Commands
{
    public class CreateDlcCommandHandler : BaseHandler, IRequestHandler<CreateDlcCommandRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DlcRules _dlcRules;

        public CreateDlcCommandHandler(DlcRules dlcRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
            : base(mapper, unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _dlcRules = dlcRules;
        }

        public async Task<Unit> Handle(CreateDlcCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _dlcRules.PriceMustBeValid(request.Price);

                await _dlcRules.GameMustExist(request.GameId);

                await _dlcRules.DlcNameMustBeUnique(request.GameId, request.Name);

                Domain.Entities.Dlc dlc = new()
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    SalePrice = request.SalePrice ?? 0,
                    ImgUrl = request.ImgUrl,
                    GameId = request.GameId
                };

                await _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().AddAsync(dlc);
                await _unitOfWork.SaveAsync();

                return Unit.Value;
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Database error occurred while saving DLC. Ensure data integrity.", ex);
            }
        }
    }
}
