using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class Basket : EntityBase
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public ICollection<BasketGame> BasketGames { get; set; } = new List<BasketGame>();
        public ICollection<BasketDlc> BasketDlcs { get; set; } = new List<BasketDlc>();
    }
}
