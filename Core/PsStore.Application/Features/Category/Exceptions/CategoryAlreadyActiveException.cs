namespace PsStore.Application.Features.Category.Exceptions
{
    public class CategoryAlreadyActiveException : Exception
    {
        public CategoryAlreadyActiveException(int categoryId)
            : base($"Category with ID '{categoryId}' is already active.") { }
    }
}
