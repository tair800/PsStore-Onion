using MediatR;

namespace PsStore.Application.Features.Dlc.Commands.CreateDlc
{
    public class CreateDlcCommandRequest : IRequest<Unit>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ImgUrl { get; set; }
        public int GameId { get; set; }


    }
}
