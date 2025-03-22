using MediatR;

namespace PsStore.Application.Features.Category.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryRequest : IRequest<Result<List<GetAllCategoriesQueryResponse>>>
    {
        public bool IncludeDeleted { get; set; } = false;
    }
}
