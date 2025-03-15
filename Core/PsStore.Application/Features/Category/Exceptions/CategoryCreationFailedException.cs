namespace PsStore.Application.Features.Category.Exceptions
{
    public class CategoryCreationFailedException : Exception
    {
        public CategoryCreationFailedException(string categoryName)
            : base($"Failed to create category '{categoryName}' due to an internal error.") { }
    }
}
