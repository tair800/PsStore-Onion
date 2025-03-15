namespace PsStore.Application.Features.Category.Exceptions
{
    public class CategoryMustHaveGamesException : Exception
    {
        public CategoryMustHaveGamesException(int categoryId)
            : base($"Category with ID '{categoryId}' must have at least one associated game.") { }
    }
}
