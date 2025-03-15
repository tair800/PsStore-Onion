namespace PsStore.Application.Features.Game.Exceptions
{
    public class GameTitleMustBeUniqueException : Exception
    {
        public GameTitleMustBeUniqueException(string title)
            : base($"A game with the title '{title}' already exists.") { }
    }
}
