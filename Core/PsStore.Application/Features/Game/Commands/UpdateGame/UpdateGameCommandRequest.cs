using MediatR;

namespace PsStore.Application.Features.Game.Commands
{
    public class UpdateGameCommandRequest : IRequest<Unit>
    {
        public int Id { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string? ImgUrl { get; set; }
        public int? CategoryId { get; set; }
        public int? PlatformId { get; set; }
        public List<int>? DlcIds { get; set; }
    }
}
