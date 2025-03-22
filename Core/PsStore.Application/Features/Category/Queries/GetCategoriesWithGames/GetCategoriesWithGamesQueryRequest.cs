using MediatR;

namespace PsStore.Application.Features.Category.Queries.GetCategoriesWithGames
{
    public class GetCategoriesWithGamesQueryRequest : IRequest<Result<List<GetCategoriesWithGamesQueryResponse>>> { }

}
