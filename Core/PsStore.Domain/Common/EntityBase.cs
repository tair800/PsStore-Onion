namespace PsStore.Domain.Common
{
    public class EntityBase : IEntityBase
    {
        public int Id { get; set; }
        public DateTime CreatedDate => DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
