using AutoMapper;
using PsStore.Application.Features.Category.Commands;
using PsStore.Application.Features.Category.Commands.DeleteCategory;
using PsStore.Application.Features.Category.Commands.RestoreCategory;
using PsStore.Application.Features.Category.Commands.UpdateCategory;
using PsStore.Application.Features.Category.Dtos;
using PsStore.Application.Features.Category.Queries.GetAllCategories;
using PsStore.Application.Features.Category.Queries.GetCategoriesWithGames;
using PsStore.Application.Features.Category.Queries.GetCategoryById;
using PsStore.Application.Features.Dlc.Commands;
using PsStore.Application.Features.Dlc.Commands.CreateDlc;
using PsStore.Application.Features.Dlc.Queries.GetAllDlc;
using PsStore.Application.Features.Dlc.Queries.GetDlcById;
using PsStore.Application.Features.Game.Commands;
using PsStore.Application.Features.Game.Commands.CreateGame;
using PsStore.Domain.Entities;

namespace PsStore.Mapper.AutoMapper.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //  Category Mappings
            CreateMap<CreateCategoryCommandRequest, Category>();
            CreateMap<UpdateCategoryCommandRequest, Category>();
            CreateMap<DeleteCategoryCommandRequest, Category>();
            CreateMap<RestoreCategoryCommandRequest, Category>();
            CreateMap<Category, GetAllCategoriesQueryResponse>();
            CreateMap<Category, GetCategoryByIdQueryResponse>();
            CreateMap<Category, GetCategoriesWithGamesQueryResponse>();

            //  DLC Mappings
            CreateMap<CreateDlcCommandRequest, Dlc>();
            CreateMap<UpdateDlcCommandRequest, Dlc>();
            CreateMap<DeleteDlcCommandRequest, Dlc>();
            CreateMap<RestoreDlcCommandRequest, Dlc>();
            CreateMap<Dlc, GetAllDlcQueryResponse>();
            CreateMap<Dlc, GetDlcByIdQueryResponse>();

            // Game Mappings
            CreateMap<Game, GameDto>();

            // Map CreateGameCommandRequest to Game
            CreateMap<CreateGameCommandRequest, Game>();

            //  Map UpdateGameCommandRequest to Game
            CreateMap<UpdateGameCommandRequest, Game>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
