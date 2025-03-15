namespace PsStore.Application.Features.Dlc.Exceptions
{
    public class DlcAlreadyDeletedException : Exception
    {
        public DlcAlreadyDeletedException(int dlcId)
            : base($"DLC with ID '{dlcId}' is already deleted.") { }
    }
}
