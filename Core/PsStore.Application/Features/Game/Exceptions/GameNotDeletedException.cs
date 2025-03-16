namespace PsStore.Application.Features.Game.Exceptions
{
    public class GameNotDeletedException : Exception
    {
        public GameNotDeletedException(int gameId)
            : base($"Game with ID {gameId} is not deleted.")
        {
        }
    }
}
