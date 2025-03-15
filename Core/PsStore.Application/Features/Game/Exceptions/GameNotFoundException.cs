namespace PsStore.Application.Features.Game.Exceptions
{
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(int gameId)
            : base($"Game with ID '{gameId}' was not found.") { }
    }
}
