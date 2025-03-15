namespace PsStore.Application.Features.Category.Exceptions
{
    public class CategoryCannotBeDeletedException : Exception
    {
        public CategoryCannotBeDeletedException(int categoryId)
            : base($"Category with ID '{categoryId}' cannot be deleted because it contains associated games.") { }
    }
}
