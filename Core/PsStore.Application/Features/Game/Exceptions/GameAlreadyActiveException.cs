namespace PsStore.Application.Features.Game.Exceptions
{
    public class GameAlreadyActiveException : Exception
    {
        public GameAlreadyActiveException(int gameId)
            : base($"Game with ID '{gameId}' is already active.") { }
    }
}
