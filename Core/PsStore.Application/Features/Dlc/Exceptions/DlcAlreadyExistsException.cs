namespace PsStore.Application.Features.Dlc.Exceptions
{
    public class DlcAlreadyExistsException : Exception
    {
        public DlcAlreadyExistsException(string dlcName, int gameId)
            : base($"A DLC with the name '{dlcName}' already exists for Game ID {gameId}.")
        {
        }
    }
}
