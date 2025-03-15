using PsStore.Application.Features.Category.Dtos;

namespace PsStore.Application.Features.Category.Queries.GetCategoriesWithGames
{
    public class GetCategoriesWithGamesQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GameDto> Games { get; set; } = new();
    }
}
