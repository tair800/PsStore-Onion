namespace PsStore.Application.Features.Basket.Queries.GetAll
{
    public class GetAllBasketsCommandResponse
    {
        public List<GetBasketResponse> Baskets { get; set; }
    }

    public class GetBasketResponse
    {
        public Guid UserId { get; set; }
        public int BasketId { get; set; }
        public List<GetBasketGameResponse> BasketGames { get; set; }
        public double TotalPrice { get; set; }
    }

    public class GetBasketGameResponse
    {
        public int BasketId { get; set; }
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public double Price { get; set; }
    }
}
