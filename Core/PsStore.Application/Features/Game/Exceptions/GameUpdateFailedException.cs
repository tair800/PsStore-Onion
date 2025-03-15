namespace PsStore.Application.Features.Game.Exceptions
{
    public class GameUpdateFailedException : Exception
    {
        public GameUpdateFailedException(int gameId)
            : base($"Failed to update Game with ID '{gameId}' due to an internal error.") { }
    }
}
