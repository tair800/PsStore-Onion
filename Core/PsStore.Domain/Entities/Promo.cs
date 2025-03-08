using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class Promo : EntityBase
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }
}
