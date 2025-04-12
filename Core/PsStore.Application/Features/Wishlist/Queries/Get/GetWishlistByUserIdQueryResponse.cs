namespace PsStore.Application.Features.Wishlist.Queries.GetWishlistByUserId
{
    public class GetWishlistByUserIdQueryResponse
    {
        public int WishlistId { get; set; }
        public List<WishlistItem> Items { get; set; }
    }

    public class WishlistItem
    {
        public int? GameId { get; set; }
        public string GameTitle { get; set; }
        public int? DlcId { get; set; }
        public string DlcName { get; set; }
    }
}
