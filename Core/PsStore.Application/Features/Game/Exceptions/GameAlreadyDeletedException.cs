namespace PsStore.Application.Features.Game.Exceptions
{
    public class GameAlreadyDeletedException : Exception
    {
        public GameAlreadyDeletedException(int gameId)
            : base($"Game with ID {gameId} is already deleted.")
        {
        }
    }
}
