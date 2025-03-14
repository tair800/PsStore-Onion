namespace PsStore.Application.Features.Category.Exceptions
{
    public class CategoryAlreadyDeletedException : Exception
    {
        public CategoryAlreadyDeletedException(int categoryId)
            : base($"Category with ID '{categoryId}' is already deleted.") { }
    }
}
