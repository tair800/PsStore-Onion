namespace PsStore.Domain.Common
{
    public interface IEntityBase
    {
        int Id { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? UpdatedDate { get; set; }
        bool IsDeleted { get; set; }
    }
}
