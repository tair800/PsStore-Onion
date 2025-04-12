namespace PsStore.Application.Features.Wishlist.Queries.GetAll
{
    public class GetAllWishlistsQueryResponse
    {
        public int WishlistId { get; set; }
        public Guid UserId { get; set; }
        public List<WishlistItems> Items { get; set; }
    }

    public class WishlistItems
    {
        public int? GameId { get; set; }
        public string GameTitle { get; set; }
        public int? DlcId { get; set; }
        public string DlcName { get; set; }
    }
}
