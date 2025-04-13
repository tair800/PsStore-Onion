namespace PsStore.Application.Features.Search.Queries
{
    public class SearchResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ImgUrl { get; set; }
        public string Type { get; set; }
    }
}
