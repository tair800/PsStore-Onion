using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class Basket : EntityBase
    {
        public Guid UserId { get; set; }  // Foreign key to the user
        public User User { get; set; }    // Navigation property for User

        public ICollection<BasketGame> BasketGames { get; set; } = new List<BasketGame>();
    }
}
