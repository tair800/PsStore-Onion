using PsStore.Domain.Common;

namespace PsStore.Domain.Entities
{
    public class Category : EntityBase
    {
        public Category()
        {

        }
        public Category(string name)
        {
            Name = name;
        }
        public string Name { get; set; }

        public ICollection<Game> Games { get; set; }
    }
}
