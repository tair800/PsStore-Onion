﻿namespace PsStore.Application.Features.Dlc.Dtos
{
    public class DlcDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public decimal SalePrice { get; set; }
        public string ImgUrl { get; set; }
        public int GameId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
