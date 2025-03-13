using AutoMapper;
using PsStore.Application.Features.Dlc.Queries.GetAllDlc;
using PsStore.Domain.Entities;

namespace PsStore.Mapper.AutoMapper.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Dlc, GetAllDlcQueryResponse>();
        }
    }
}
