namespace PsStore.Application.Features.Basket.Queries.GetBasket
{
    public class GetBasketCommandResponse
    {
        public Guid UserId { get; set; }
        public List<GetBasketGameResponse> BasketGames { get; set; } = new List<GetBasketGameResponse>();
        public double TotalPrice { get; set; }
    }

    public class GetBasketGameResponse
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public double Price { get; set; }
    }
}
