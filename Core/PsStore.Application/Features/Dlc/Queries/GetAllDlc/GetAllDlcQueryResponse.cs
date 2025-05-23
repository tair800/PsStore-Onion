﻿using PsStore.Application.Common;
using System.Text.Json.Serialization;

namespace PsStore.Application.Features.Dlc.Queries.GetAllDlc
{
    public class GetAllDlcQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public decimal SalePrice { get; set; }
        public string ImgUrl { get; set; }
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public bool IsDeleted { get; set; }

        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime CreatedDate { get; set; }

        [JsonConverter(typeof(NullableJsonDateTimeConverter))]
        public DateTime? UpdatedDate { get; set; }
    }
}
