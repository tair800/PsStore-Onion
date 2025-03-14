using AutoMapper;
using PsStore.Application.Features.Category.Commands.UpdateCategory;
using PsStore.Application.Features.Dlc.Commands;
using PsStore.Application.Features.Dlc.Queries.GetAllDlc;
using PsStore.Application.Features.Dlc.Queries.GetDlcById;
using PsStore.Domain.Entities;

namespace PsStore.Mapper.AutoMapper.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Dlc, GetAllDlcQueryResponse>();
            CreateMap<Dlc, GetDlcByIdQueryResponse>();
            CreateMap<UpdateDlcCommandRequest, Dlc>();
            CreateMap<UpdateCategoryCommandRequest, Category>();

        }
    }
}
