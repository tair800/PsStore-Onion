using MediatR;

namespace PsStore.Application.Features.Game.Commands
{
    public class CreateGameCommandRequest : IRequest<Unit>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ImgUrl { get; set; }
        public int CategoryId { get; set; }
        public int PlatformId { get; set; }
    }
}
