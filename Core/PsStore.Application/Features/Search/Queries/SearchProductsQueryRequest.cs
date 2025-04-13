using MediatR;

namespace PsStore.Application.Features.Search.Queries
{
    public class SearchProductsQueryRequest : IRequest<Result<List<SearchResult>>>
    {
        public string Keyword { get; set; }
    }
}
