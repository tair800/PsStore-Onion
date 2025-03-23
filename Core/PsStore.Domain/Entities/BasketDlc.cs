using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class BasketDlc : EntityBase
    {
        public int BasketId { get; set; }
        public Basket Basket { get; set; }

        public int DlcId { get; set; }
        public Dlc Dlc { get; set; }

        public double Price { get; set; }
    }
}
