using MediatR;
using PsStore.Application.Features.Dlc.Dtos;
using PsStore.Application.Features.Dlc.Exceptions;
using PsStore.Application.Interfaces.UnitOfWorks;

namespace PsStore.Application.Features.Dlc.Queries
{
    public class GetDlcByIdQueryHandler : IRequestHandler<GetDlcByIdQuery, DlcDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDlcByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DlcDto> Handle(GetDlcByIdQuery request, CancellationToken cancellationToken)
        {
            var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAsync(d => d.Id == request.Id);
            if (dlc == null)
            {
                throw new DlcNotFoundException(request.Id);
            }

            return new DlcDto
            {
                Id = dlc.Id,
                Name = dlc.Name,
                Description = dlc.Description,
                Price = dlc.Price,
                SalePrice = dlc.SalePrice,
                ImgUrl = dlc.ImgUrl,
                GameId = dlc.GameId,
                IsDeleted = dlc.IsDeleted
            };
        }
    }
}
