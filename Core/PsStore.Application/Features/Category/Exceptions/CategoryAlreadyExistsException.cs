namespace PsStore.Application.Features.Category.Exceptions
{
    public class CategoryAlreadyExistsException : Exception
    {
        public CategoryAlreadyExistsException(string categoryName)
            : base($"A category with the name '{categoryName}' already exists.") { }
    }
}
