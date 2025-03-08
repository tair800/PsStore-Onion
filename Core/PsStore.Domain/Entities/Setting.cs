using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class Setting : EntityBase
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
