using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class Wishlist : EntityBase
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
