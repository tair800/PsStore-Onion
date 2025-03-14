namespace PsStore.Application.Features.Category.Exceptions
{
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException(int categoryId)
            : base($"Category with ID '{categoryId}' was not found.") { }
    }
}
