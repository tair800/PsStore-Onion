using PsStore.Domain.Common;
using PsStore.Domain.Entities;

public class WishlistItem : EntityBase
{
    public int? GameId { get; set; }
    public Game Game { get; set; }
    public int WishlistId { get; set; }
    public Wishlist Wishlist { get; set; }

    public int? DlcId { get; set; }
    public Dlc Dlc { get; set; }
}