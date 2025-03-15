public class CategoryDeleteFailedException : Exception
{
    public CategoryDeleteFailedException(int categoryId)
        : base($"An unexpected error occurred while deleting category with ID {categoryId}.") { }
}
