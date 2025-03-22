using MediatR;

namespace PsStore.Application.Features.Category.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryRequest : IRequest<Result<GetCategoryByIdQueryResponse>>
    {
        public int Id { get; set; }
    }
}
