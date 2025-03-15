using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class Game : EntityBase
    {
        public Game()
        {

        }
        public Game(string title, string description, double price, decimal salePrice, string imgUrl, int categoryId)
        {
            Title = title;
            Description = description;
            Price = price;
            SalePrice = salePrice;
            ImgUrl = imgUrl;
            CategoryId = categoryId;
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public decimal? SalePrice { get; set; }

        public string ImgUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public Platform Platform { get; set; }
        public ICollection<Dlc>? Dlcs { get; set; } = new List<Dlc>();
        //public ICollection<WishlistGame> WishlistGames { get; set; } = new List<WishlistGame>();
        //public ICollection<BasketGame> BasketGames { get; set; }
        //public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Rating>? Ratings { get; set; }
    }
}
