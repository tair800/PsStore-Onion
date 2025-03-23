using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class BasketGame : EntityBase
    {
        public int GameId { get; set; }
        public Game Game { get; set; }

        public int BasketId { get; set; }
        public Basket Basket { get; set; }

        public double Price { get; set; }

        public double TotalPrice => Price;


    }
}
