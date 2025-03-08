using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class Dlc : EntityBase
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public int GameId { get; set; }

        public Game Game { get; set; }
        //public ICollection<BasketGame> BasketGames { get; set; }
    }
}
