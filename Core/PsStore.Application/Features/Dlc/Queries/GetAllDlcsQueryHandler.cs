using MediatR;
using PsStore.Application.Features.Dlc.Dtos;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Queries
{
    public class GetAllDlcsQueryHandler : IRequestHandler<GetAllDlcsQuery, IList<DlcDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllDlcsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<DlcDto>> Handle(GetAllDlcsQuery request, CancellationToken cancellationToken)
        {
            var dlcs = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>()
                .GetAllAsync(includeDeleted: request.IncludeDeleted);

            return dlcs.Select(dlc => new DlcDto
            {
                Id = dlc.Id,
                Name = dlc.Name,
                Description = dlc.Description,
                Price = dlc.Price,
                SalePrice = dlc.SalePrice,
                ImgUrl = dlc.ImgUrl,
                GameId = dlc.GameId,
                IsDeleted = dlc.IsDeleted
            }).ToList();
        }
    }
}
