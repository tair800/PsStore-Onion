namespace PsStore.Application.Features.Basket.Queries.GetAllBaskets
{
    public class GetAllBasketsCommandResponse
    {
        public List<GetAllBasketsCommandResponse> Baskets { get; set; } // This will hold the list of basket responses
        public Guid UserId { get; set; }
        public int BasketId { get; set; }
        public List<GetBasketGameResponse> BasketGames { get; set; } = new List<GetBasketGameResponse>();
        public double TotalPrice { get; set; }
    }


    public class GetBasketGameResponse
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public double Price { get; set; }
        public List<GetBasketDlcResponse> Dlcs { get; set; } = new List<GetBasketDlcResponse>();
    }

    public class GetBasketDlcResponse
    {
        public int DlcId { get; set; }
        public string DlcName { get; set; }
        public double DlcPrice { get; set; }
    }
}
