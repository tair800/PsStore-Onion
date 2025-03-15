public class GameCreationFailedException : Exception
{
    public GameCreationFailedException(string title)
        : base($"Failed to create game '{title}' due to an internal error.") { }
}
