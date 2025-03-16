using PsStore.Application.Common;
using PsStore.Application.Features.Game.Dtos;
using System.Text.Json.Serialization;

namespace PsStore.Application.Features.Game.Queries.GetAllGame
{
    public class GetAllGameQueryResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ImgUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string PlatformName { get; set; }
        public List<DlcDto> Dlcs { get; set; } = new();

        public bool IsDeleted { get; set; }

        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime CreatedDate { get; set; }

        [JsonConverter(typeof(NullableJsonDateTimeConverter))]
        public DateTime? UpdatedDate { get; set; }
    }


}
