using AutoMapper;
using PsStore.Application.Features.Category.Commands.UpdateCategory;
using PsStore.Application.Features.Category.Dtos;
using PsStore.Application.Features.Category.Queries.GetAllCategories;
using PsStore.Application.Features.Category.Queries.GetCategoriesWithGames;
using PsStore.Application.Features.Category.Queries.GetCategoryById;
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
            CreateMap<Category, GetAllCategoriesQueryResponse>();
            CreateMap<Category, GetCategoryByIdQueryResponse>();
            CreateMap<Category, GetCategoriesWithGamesQueryResponse>();
            CreateMap<Game, GameDto>();

        }
    }
}
