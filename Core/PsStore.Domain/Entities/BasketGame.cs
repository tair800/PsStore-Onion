using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class BasketGame : EntityBase
    {
        public int GameId { get; set; }
        public Game Game { get; set; }

        public int BasketId { get; set; }
        public Basket Basket { get; set; }

        public double Price { get; set; }   // Price of the game in the basket

        public double TotalPrice => Price;  // Calculate total price of the game in the basket (price for now)
    }
}
