namespace PsStore.Application.Features.Dlc.Exceptions
{
    public class DlcAlreadyActiveException : Exception
    {
        public DlcAlreadyActiveException(int dlcId)
            : base($"DLC with ID '{dlcId}' is already active.") { }
    }
}
