namespace PsStore.Application.Features.Game.Exceptions
{
    public class GameTitleAlreadyExistsException : Exception
    {
        public GameTitleAlreadyExistsException(string title)
            : base($"A game with the title '{title}' already exists.") { }
    }
}
