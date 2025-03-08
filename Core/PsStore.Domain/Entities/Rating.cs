using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class Rating : EntityBase
    {
        public int GameId { get; set; }
        public Game Game { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public int Score { get; set; }
    }
}
