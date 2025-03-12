//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using PsStore.Application.Bases;
//using PsStore.Application.Features.Dlc.Exceptions;
//using PsStore.Application.Features.Dlc.Rules;
//using PsStore.Application.Interfaces.AutoMapper;
//using PsStore.Application.Interfaces.UnitOfWorks;
//using PsStore.Domain.Entities;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace PsStore.Application.Features.Dlc.Commands
//{
//    public class UpdateDlcCommandHandler : BaseHandler, IRequestHandler<UpdateDlcCommandRequest, Unit>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly DlcRules _dlcRules;

//        public UpdateDlcCommandHandler(DlcRules dlcRules, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
//            : base(mapper, unitOfWork, httpContextAccessor)
//        {
//            _unitOfWork = unitOfWork;
//            _dlcRules = dlcRules;
//        }

//        public async Task<Unit> Handle(UpdateDlcCommandRequest request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var dlc = await _unitOfWork.GetReadRepository<Domain.Entities.Dlc>().GetAsync(d => d.Id == request.Id);
//                if (dlc == null)
//                {
//                    throw new DlcNotFoundException(request.Id);
//                }

//                await _dlcRules.GameMustExist(request.GameId);

//                if (dlc.Name != request.Name)
//                {
//                    await _dlcRules.DlcNameMustBeUnique(request.GameId, request.Name);
//                }

//                _dlcRules.PriceMustBeValid(request.Price);

//                dlc.Name = request.Name;
//                dlc.Description = request.Description;
//                dlc.Price = request.Price;
//                dlc.SalePrice = request.SalePrice ?? 0;
//                dlc.ImgUrl = request.ImgUrl;
//                dlc.GameId = request.GameId;

//                _unitOfWork.GetWriteRepository<Domain.Entities.Dlc>().Update(dlc);
//                await _unitOfWork.SaveAsync();

//                return Unit.Value;
//            }
//            catch (DbUpdateException ex)
//            {
//                throw new DbUpdateException("Database error occurred while updating DLC. Ensure data integrity.", ex);
//            }
//        }
//    }
//}
